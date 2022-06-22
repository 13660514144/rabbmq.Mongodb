
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
            //�˴�����ע��̳��ԣ�ICapSubscribe�ӿڵĶ��ķ�������CapUserService��̳���ICapSubscribe�ӿ�
            //�̳���ICapSubscribe�ӿڵĶ��ģ���Ҫ�ڣ�AddCap����֮ǰע����񣬷��򽫲��ᱻɨ�赽
            services.AddSingleton<GetAppsetting>();
            services.AddTransient<ICapUserService, CapUserService>();
            services.AddSingleton<Appsetings>();
            //var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            //����������ڿ���֧��ʹ��EntityFramework��ʹ�ô˷���ʱ����������UseSqlServer����Mysql

            //services.AddDbContext<AppDbContext>();

            //����Cap

            services.AddCap(x =>
            {
                //����Cap�ı�����Ϣ��¼�⣬���ڷ���˱���Published��Ϣ��¼���ͻ��˱���Received��Ϣ��¼��

                // �˷���Ĭ��ʹ�õ����ݿ�SchemaΪCap��2��Ҫ�����sql server2012(��Ϊʹ����Dashboard��sql��ѯ���ʹ����Format�º���)
                //x.UseSqlServer("Data Source=LAPTOP-LGF126KE\\VERITRAX;Initial Catalog=Cap;User ID=sa;Password=Bod33|en17?ha!;Integrated Security=false;");
                //x.UseMongoDB("mongodb://localhost:27017/cap?authSource=cap");
                //x.DefaultGroupName = "Lxzpower.Cap.RabbMq.Queue";
                x.UseMongoDB(Configuration["MongoDb:mongodb_Connection"]);
                // ����Cap�ı�����Ϣ��¼�⣬���ڷ���˱���Published��Ϣ��¼���ͻ��˱���Received��Ϣ��¼��
                // �˷�������ָ���Ƿ�ʹ��sql server2008,���ݿ�Schema,�����ַ���
                /*x.UseSqlServer((options) =>
                {
                    //���ݿ������ַ���
                    options.ConnectionString = "Integrated Security=False;server=192.168.1.109;database=cap;User ID=sa;Password=����;Connect Timeout=30";
                    //���ʹ�õ���SqlServer2008����(�˴����õ���2008,��Ϊ192.168.1.109���ݿ���2008)
                    options.UseSqlServer2008();
                    //CapĬ��ʹ�õ����ݿ�SchemaΪCap;�˴�����ָ��ʹ���Լ������ݿ�Schema
                    //options.Schema = "dbo";
                });*/

                //ʹ��Kafka��Ϊ�ײ�֮�����Ϣ����
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

                //ʹ��Dashboard������һ��Cap�Ŀ��ӻ�������棻Ĭ�ϵ�ַ:http://localhost:�˿�/cap
                x.UseDashboard();

                //Ĭ�Ϸ���������ֵ������ʱ��Ĭ��ֵΪ��ǰ���򼯵�����
                //x.DefaultGroup = "m";
                //ʧ�ܺ�����Դ�����Ĭ��50�Σ���FailedRetryIntervalĬ��60�������£���Ĭ������50*60��(50����)֮�����ʧ������
                x.FailedRetryCount = Convert.ToInt32(Configuration["RabbitMQ:FailedRetryCount"]);

                //ʧ�ܺ����ʰ�����Ĭ��60��
                x.FailedRetryInterval = Convert.ToInt32(Configuration["RabbitMQ:FailedRetryInterval"]);

                //���óɹ���Ϣ��ɾ��ʱ��Ĭ��24*3600��
                x.SucceedMessageExpiredAfter = Convert.ToInt32(Configuration["RabbitMQ:SucceedMessageExpiredAfter"]);
            });
            
            //��������
            services.AddTransient<ISubscriberService, SubscriberService>();
            services.AddTransient<ISubscriberListService, SubscriberListService>();
            //��������

            //
            
            services.AddSingleton<ApiQueue>();
            services.AddSingleton<HangJobMethod>();
            services.AddSingleton<WebSocketService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //ȫ�ִ������
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
            // ������̬ҳ��
            DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            defaultFilesOptions.DefaultFileNames.Clear();
            defaultFilesOptions.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(defaultFilesOptions);
            //app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot")),
                // ����Linux ʹ��
                /*FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))*/
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //����cap�м��
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


            //������Ĵ�Ȩ�ޡ����������˶����Դ���塣���Բ�����̨����
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //����1
                Authorization = new[]
               {
                 new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                 {
                    SslRedirect = false,          // �Ƿ����з�SSL�����ض���SSL URL
                    RequireSsl = false,           // ��ҪSSL���Ӳ��ܷ���HangFire Dahsboard��ǿ�ҽ�����ʹ�û��������֤ʱʹ��SSL
                    LoginCaseSensitive = false,   //��¼����Ƿ����ִ�Сд
                    Users = new[]
                    {
                        new BasicAuthAuthorizationUser
                        {
                            Login ="LZD",//�û���
                            PasswordClear="123456"
                            // Password as SHA1 hash
                            //Password=new byte[]{ 0xf3,0xfa����0xd1 }
                        }
                    }
                 })
               },

            });

            
            

            var _Hangfire = MyServiceProvider.ServiceProvider.GetRequiredService<HangJobMethod>();
            //var BackServer = new BackgroundJobServer();
            //var jobId = BackgroundJob.Enqueue(() => Console.WriteLine("{0}===�����Ƕ�������!", DateTime.Now.ToString("HH:mm:ss")));
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
