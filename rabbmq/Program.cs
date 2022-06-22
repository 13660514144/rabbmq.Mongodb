
using BeetleX.XRPC.Hosting;
using BeetleX.XRPC.Packets;
using HelperTools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xrpc_Beetlx;

namespace rabbmq
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            //var refAssembyNames=Assembly.Load("rabbmq.XrpcInterface").GetTypes();
            /*Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "rabbmq.XrpcInterface");
            List<Assembly> Ass=new List<Assembly>();
            foreach (var asslembyNames in typelist)
            {
                Assembly.Load(asslembyNames.Name);
                //Assembly a = typeof(asslembyNames).Assembly;
                Console.WriteLine(asslembyNames.Name);
            }*/
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> Ass = new List<Assembly>();
            Ass.Add(typeof(I_sampleCap).Assembly);
            /*TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long tt = Convert.ToInt64(ts.TotalMilliseconds);
            Console.WriteLine($"tt={tt}");
            Console.WriteLine($"{TimeSpans.Timestamp()}");*/
            CreateHostBuilder(args, Ass.ToArray()).Build().Run();
        }
        /*private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }*/

        public static IHostBuilder CreateHostBuilder(string[] args, Assembly[] assemblies) =>
            Host.CreateDefaultBuilder(args) //配置log4Net配置文件            
            /*.ConfigureServices((hostContext, services) =>
            {
                services.UseXRPC(s =>
                {
                    s.ServerOptions.LogLevel = BeetleX.EventArgs.LogType.Trace;
                    s.ServerOptions.DefaultListen.Port = 9090;
                    s.RPCOptions.ParameterFormater = new JsonPacket();//default messagepack
                }, assemblies);
            })*/
            .ConfigureLogging(loggingBuilder => { loggingBuilder.AddLog4Net("log4net.Config"); })            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.AllowSynchronousIO = true;//启用同步 IO                        
                    })
                    .ConfigureAppConfiguration(builder =>
                    {
                        builder.AddJsonFile("hosting.json", optional: true);
                    });
                });
    }    
}
