using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DotNetCore.CAP;
using Microsoft.Extensions.Logging;
using rabbmq.Class;
using HelperTools;

namespace rabbmq.Signalr
{
    public class SignAlrHub:Hub
    {
        private string GUID;
        private ICapPublisher _IbusPublish;
        private ResponeTo Rep = new ResponeTo();
        private dynamic Reqparams = new DynamicObj();
        private readonly ILogger<SignAlrHub> _logger;
        public SignAlrHub(ICapPublisher capPublish, ILogger<SignAlrHub> logger)
        {
            _IbusPublish = capPublish;
            _logger = logger;
        }
        /// <summary>
        /// 连接事件
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {            
            await base.OnConnectedAsync();
            string connectionID = Context.ConnectionId;
            //查询用户
            var user = ChatUser.users.SingleOrDefault(u => u.ConnectionID == connectionID);
            //判断用户是否存在，否则添加集合
            if (user == null)
            {
                user = new SignAlrUser(Context.ConnectionId, new Guid().ToString(), "已绑定用户",string.Empty);
                user.Clients = Clients;
                user.Context = Context;
                ChatUser.users.Add(user);
            }
            //await Clients.All.SendAsync("ConnectMessage", "用户上线", connectionID);
            SendMsg msg = new SendMsg
            {
                SendType = "RequestLink",
                Sendcode = 200,
                Message = "连接成功",
                Data = new JArray()
            };
            await Clients.Client(connectionID).SendAsync("ConnectMessage", JsonConvert.SerializeObject(msg), connectionID);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connectionID = Context.ConnectionId;
            //查询用户
            var user = ChatUser.users.SingleOrDefault(u => u.ConnectionID == connectionID);
            //判断用户是否存在，否则添加集合
            if (user != null)
            {
                ChatUser.users.Remove(user);
            }
            //await Clients.All.SendAsync("ConnectMessage", "用户" + connectionID + "由于" + exception?.Message + "掉线");

            await base.OnDisconnectedAsync(exception);
        }
        /// <summary>
        /// 远程设备连接
        /// </summary>
        /// <param name="seekSocket"></param>
        /// <returns>是否存在错误数据</returns>
        public async Task<bool> Link(string ClientType)
        {
            bool Flg = true;
            string connectionID = Context.ConnectionId;
            //查询用户
            var user = ChatUser.users.SingleOrDefault(u => u.ConnectionID == connectionID);
            if (user != null)
            {
                user.Type = ClientType;
            }
            else
            {
                user = new SignAlrUser(Context.ConnectionId, new Guid().ToString(), "已绑定用户", ClientType);
                user.Clients = Clients;
                user.Context = Context;
                ChatUser.users.Add(user);
            }
            return Flg;
        }
        public async Task<string> GetListPage(string Json)
        {
            string Result = string.Empty;
            GUID = Guid.NewGuid().ToString("N");
            string PubName = "ListPage.Collection";
            // 加入队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams = Rep.CreatPub(GUID, $"{Json}");
            // 发布业务        
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            var ToClient = await Rep.ResChangeReturn(GUID);
            return JsonConvert.SerializeObject(ToClient);
        }
        public class SendMsg
        { 
            public string SendType { get; set; }
            public int Sendcode { get; set; }
            public string Message { set; get; }
            public object Data { get; set; }
        }
    }
}
