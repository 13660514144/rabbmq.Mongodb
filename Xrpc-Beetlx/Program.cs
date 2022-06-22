using BeetleX.XRPC.Hosting;
using BeetleX.XRPC.Packets;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MongodbLibrary;
using Newtonsoft.Json;

namespace Xrpc_Beetlx
{
    class Program
    {
        protected static IServiceProvider m_Service;
        static void Main(string[] args)
        {
            var serviceArr = new ServiceCollection();
            serviceArr.AddSingleton<GetMongoAppsetting>();
            serviceArr.AddSingleton<Appsetings>();
            m_Service = serviceArr.BuildServiceProvider();
            MongoServiceProvider.ServiceProvider = m_Service;
           
            var refAssembyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            foreach (var asslembyNames in refAssembyNames)
            {
                Assembly.Load(asslembyNames);
            }
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.UseXRPC(s =>
                {
                    s.ServerOptions.LogLevel = BeetleX.EventArgs.LogType.Trace;
                    s.ServerOptions.DefaultListen.Port = 9090;
                    s.RPCOptions.ParameterFormater = new JsonPacket();//default messagepack
                }, assemblies);
                
            });
            builder.Build().Run();
            
        }
    }
}
