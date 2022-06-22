using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.WebSockets;
using System.Threading.Tasks;
using Coldairarrow.DotNettySocket;
using HelperTools;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using rabbmq.AssemblyRegister;
using rabbmq.Class;
using rabbmq.Dbclass;

namespace rabbmq.NettySocket
{
    public class WebSocketService
    {
        public IWebSocketServer theServer;
        public WebSocketNetty _WebSocket;
        public List<WebSocketNetty> ListSocket;
        public WebSocketService()
        { 
        }
        public async void WsSocket_Init()
        {
            await WebSocketInit();

        }
        public async Task WebSocketInit()
        {
            var config = MyServiceProvider.ServiceProvider.GetRequiredService<GetAppsetting>();
            try
            {
                int Port = Convert.ToInt32(config.ConfigAppset.GetSection("SocketPort").Value);
                theServer = await SocketBuilderFactory.GetWebSocketServerBuilder(Port)
                    .OnConnectionClose((server, connection) =>
                    {                                                
                        Task.Run(async ()=> {
                            await InitClosed(connection.ConnectionId);
                        });
                        LogMongdHelper.InfoLog("WebSocketService", "WebSocketInit", "OnException", $"连接关闭:{connection.ConnectionId}");
                    })
                    .OnException(ex =>
                    {
                        LogMongdHelper.ErrorLog(ex, "WebSocketService", "WebSocketInit", "OnException");
                    })
                    .OnNewConnection((server, connection) =>
                    {
                        connection.ConnectionName = $"{connection.ConnectionId}";                        
                        Task.Run(async ()=> {
                            await InitConnectId(connection.ConnectionId);
                        });
                        LogMongdHelper.InfoLog("WebSocketService", "WebSocketInit", "OnNewConnection", $"建立连接:{connection.ConnectionId}");
                    })
                    .OnRecieve((server, connection, msg) =>
                    {
                        Task.Run(async ()=> {
                            await InitSeverice(connection.ConnectionId,msg);
                        });
                    })
                    .OnSend((server, connection, msg) =>
                    {
                        //Console.WriteLine($"向连接名[{connection.ConnectionName}]发送数据:{msg}");
                    })
                    .OnServerStarted(server =>
                    {
                        LogMongdHelper.InfoLog("WebSocketService", "WebSocketInit", "OnServerStarted", $"WS-Socket服务启动");

                    }).BuildAsync();
            }
            catch (Exception ex)
            {                
                LogMongdHelper.ErrorLog(ex, "WebSocketService", "WebSocketInit", "try catch");
            }
            //Console.ReadLine();
        }
        /// <summary>
        /// 初始化连接模块
        /// </summary>
        /// <param name="ConnectId"></param>
        /// <returns></returns>
        public async Task InitConnectId(string ConnectId)
        {
            _WebSocket = new WebSocketNetty();
            _WebSocket._id = ObjectId.GenerateNewId();
            _WebSocket.ConnectId = ConnectId;
            _WebSocket.Timespan= TimeSpans.Timestamp();
            _WebSocket.Type = "app";

            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
            MongoDbDal<WebSocketNetty>.GuestSetHost(MongoUrl, "ownerconfig", "socketCollection");
            await MongoDbDal<WebSocketNetty>.InsertOneAsync(_WebSocket);
        }
        public async Task InitSeverice(string ConnectId,string Msg)
        {
            JObject O = JObject.Parse(Msg);
            switch ((string)O["Mothed"])
            {
                case "Heart":
                case "ReturnClientModel":
                    JObject Socket = (JObject)O["Data"];
                    WebSocketNetty Usocket = new WebSocketNetty
                    {
                        Ip=Socket["ClientIp"].ToString(),
                        Port= Socket["ClientPort"].ToString(),
                        Type= Socket["DeviceType"].ToString(),
                        Timespan = TimeSpans.Timestamp(),
                    };
                    var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
                    string MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
                    MongoDbDal<WebSocketNetty>.GuestSetHost(MongoUrl, "ownerconfig", "socketCollection");
                    Dictionary<string, object> D = new Dictionary<string, object>
                    {
                        { "ConnectId",$@"{ConnectId}"}
                    };
                    await MongoDbDal<WebSocketNetty>.UpdateFieldAsync(Usocket, D);
                    break;
                //case "Heart":
                //    break;
            }
        }
        public async Task InitClosed(string ConnectId)
        {
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/socketCollection";
            MongoDbDal<object>.GuestSetHost(MongoUrl, "ownerconfig", "socketCollection");
            Dictionary<string, object> D = new Dictionary<string, object>
            {
                { "ConnectId",$@"{ConnectId}"}
            };
            await MongoDbDal<WebSocketNetty>.DeleteFieldAsync(D);
        }
        /// <summary>
        public async void SeverTOClient(IWebSocketConnection Client, string Msg)
        {
            await Client.Send(Msg);
        }
        public class DisbSend
        {
            public IWebSocketConnection ConnectionId { get; set; }
            public string Mothed { get; set; }
            public object data { get; set; } = new object();
            public string id { get; set; }
            public long time { get; set; }
            public string sourceReq { get; set; }
        }
        public class Wsmodel
        {
            public string ConnectId { get; set; }
            public string ClientIp { get; set; }
            public string ClientPort { get; set; }
            public string DeviceType { get; set; }
        }        

    }
}
