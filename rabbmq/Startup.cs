
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.MemoryStorage;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HelperTools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using rabbmq.Class;
using rabbmq.Dbclass;
using rabbmq.HangFireJob;
using rabbmq.NettySocket;
using rabbmq.Signalr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using static rabbmq.Class.ListPaging;
using static rabbmq.Class.TestSubscriber;

namespace rabbmq
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region hangfire
            /*var mongoUrlBuilder = new MongoUrlBuilder("mongodb://localhost:27017/jobss");
            var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
            // Add Hangfire services. Hangfire.AspNetCore nuget required
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, new MongoStorageOptions
                {
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy()
                    },
                    Prefix = "hangfire.mongo",
                    CheckConnection = true
                })
            );
            // Add the processing server as IHostedService
            services.AddHangfireServer(serverOptions =>
            {
                serverOptions.ServerName = "Hangfire.Mongo server 1";
            });*/
            services.AddHangfire(x => x.UseStorage(new MemoryStorage()));
            #endregion
            // Add framework services.
            services.AddControllers();
            services.AddSignalR().AddHubOptions<SignAlrHub>(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024;// long.MaxValue;                
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.EnableDetailedErrors = true;
            });
            //此处用于注册继承自：ICapSubscribe接口的订阅服务，以下CapUserService类继承了ICapSubscribe接口
            //继承自ICapSubscribe接口的订阅，需要在：AddCap方法之前注册服务，否则将不会被扫描到
            services.AddSingleton<GetAppsetting>();
            services.AddTransient<ICapUserService, CapUserService>();
            services.AddSingleton<Appsetings>();
            //var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            //下面语句用于开启支持使用EntityFramework，使用此方案时，无需配置UseSqlServer或者Mysql

            //services.AddDbContext<AppDbContext>();

            //配置Cap

            services.AddCap(x =>
            {
                //配置Cap的本地消息记录库，用于服务端保存Published消息记录表；客户端保存Received消息记录表

                // 此方法默认使用的数据库Schema为Cap；2，要求最低sql server2012(因为使用了Dashboard的sql查询语句使用了Format新函数)
                //x.UseSqlServer("Data Source=LAPTOP-LGF126KE\\VERITRAX;Initial Catalog=Cap;User ID=sa;Password=Bod33|en17?ha!;Integrated Security=false;");
                //x.UseMongoDB("mongodb://localhost:27017/cap?authSource=cap");
                //x.DefaultGroupName = "Lxzpower.Cap.RabbMq.Queue";
                x.UseMongoDB(Configuration["MongoDb:mongodb_Connection"]);
                // 配置Cap的本地消息记录库，用于服务端保存Published消息记录表；客户端保存Received消息记录表
                // 此方法可以指定是否使用sql server2008,数据库Schema,链接字符串
                /*x.UseSqlServer((options) =>
                {
                    //数据库连接字符串
                    options.ConnectionString = "Integrated Security=False;server=192.168.1.109;database=cap;User ID=sa;Password=密码;Connect Timeout=30";
                    //标记使用的是SqlServer2008引擎(此处设置的是2008,因为192.168.1.109数据库是2008)
                    options.UseSqlServer2008();
                    //Cap默认使用的数据库Schema为Cap;此处可以指定使用自己的数据库Schema
                    //options.Schema = "dbo";
                });*/

                //使用Kafka作为底层之间的消息发送
                //x.UseKafka("192.168.1.230:9092,192.168.1.231:9092,192.168.1.232:9092");
                /*x.UseRabbitMQ(rb => {
                    rb.HostName = "localhost";
                    rb.UserName = "guest";
                    rb.Password = "guest";
                    rb.Port = 5672;
                    rb.VirtualHost = "/";
                });*/
                x.UseRabbitMQ(rb => {
                    rb.HostName = Configuration["RabbitMQ:HostName"];
                    rb.UserName = Configuration["RabbitMQ:UserName"];
                    rb.Password = Configuration["RabbitMQ:Password"];
                    rb.ExchangeName= Configuration["ExchangeName"];
                    rb.Port = Convert.ToInt32(Configuration["RabbitMQ:Port"]);
                    rb.VirtualHost = Configuration["RabbitMQ:VirtualHost"];
                });
                //x.UseKafka(options =>
                //{
                //    options.Servers = "192.168.1.230:9092,192.168.1.231:9092,192.168.1.232:9092";
                //});

                //使用Dashboard，这是一个Cap的可视化管理界面；默认地址:http://localhost:端口/cap
                x.UseDashboard();

                //默认分组名，此值不配置时，默认值为当前程序集的名称
                //x.DefaultGroup = "m";
                //失败后的重试次数，默认50次；在FailedRetryInterval默认60秒的情况下，即默认重试50*60秒(50分钟)之后放弃失败重试
                x.FailedRetryCount = Convert.ToInt32(Configuration["RabbitMQ:FailedRetryCount"]);

                //失败后的重拾间隔，默认60秒
                x.FailedRetryInterval = Convert.ToInt32(Configuration["RabbitMQ:FailedRetryInterval"]);

                //设置成功信息的删除时间默认24*3600秒
                x.SucceedMessageExpiredAfter = Convert.ToInt32(Configuration["RabbitMQ:SucceedMessageExpiredAfter"]);
            });
            
            //发布订阅
            services.AddTransient<ISubscriberService, SubscriberService>();
            services.AddTransient<ISubscriberListService, SubscriberListService>();
            //发布订阅

            //
            
            services.AddSingleton<ApiQueue>();
            services.AddSingleton<HangJobMethod>();
            services.AddSingleton<WebSocketService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //全局错误跟踪
            app.UseMiddleware(typeof(CustomExceptionMiddleware));
            #region 
            /* app.UseExceptionHandler(config =>
             {
                 config.Run(async context =>
                 {
                     context.Response.StatusCode = 500;
                     context.Response.ContentType = "application/json";

                     var error = context.Features.Get<IExceptionHandlerFeature>();
                     if (error != null)
                     {
                         var ex = error.Error;
                         var body = JsonSerializer.Serialize(new
                         {
                             StatusCode = 500,
                             message = ex.Message,
                             stackTrace =  ex.StackTrace 
                         });
                         await context.Response.WriteAsync(body);
                     }
                 });
             });*/
            #endregion 
            // 开启静态页面
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),
                // 下面Linux 使用
                /*FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))*/
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //启用cap中间件
            MyServiceProvider.ServiceProvider = app.ApplicationServices;

            /*var options = new MongoStorageOptions
            {
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                }
            };
            GlobalConfiguration.Configuration.UseMongoStorage("mongodb://localhost/jobss", options);*/
            app.UseHangfireServer();
            //app.UseHangfireDashboard();


            //添加面板的打开权限。不是所有人都可以打开面板。可以操作后台任务。
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //方法1
                Authorization = new[]
               {
                 new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                 {
                    SslRedirect = false,          // 是否将所有非SSL请求重定向到SSL URL
                    RequireSsl = false,           // 需要SSL连接才能访问HangFire Dahsboard。强烈建议在使用基本身份验证时使用SSL
                    LoginCaseSensitive = false,   //登录检查是否区分大小写
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login ="LZD",//用户名
                            PasswordClear="123456"
                            // Password as SHA1 hash
                            //Password=new byte[]{ 0xf3,0xfa，，0xd1 }
                        }
                    }
                 })
               },

            });

            
            

            var _Hangfire = MyServiceProvider.ServiceProvider.GetRequiredService<HangJobMethod>();
            //var BackServer = new BackgroundJobServer();
            //var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("{0}===》这是队列任务!", DateTime.Now.ToString("HH:mm:ss")));
            RecurringJob.AddOrUpdate("Job CleanLogFile", () => _Hangfire.CleanLogFile(), Cron.Daily(12,1));
            RecurringJob.AddOrUpdate("Job CleanQueApi", () => _Hangfire.CleanQueApi(), "*/59 * * * * *", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate("Job CleanSocket", () => _Hangfire.ClearSocket(), "*/59 * * * * *", TimeZoneInfo.Local);
            var _WsSocket = MyServiceProvider.ServiceProvider.GetRequiredService<WebSocketService>();
            _WsSocket.WsSocket_Init();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignAlrHub>("/signalr");
            });           
        }
    }
}
