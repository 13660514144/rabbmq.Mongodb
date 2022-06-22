using DotNetCore.CAP;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using rabbmq.Dbclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using rabbmq.Class;
using HelperTools;
using Microsoft.Extensions.Logging;


namespace rabbmq.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private string GUID;
        private ICapPublisher _IbusPublish;
        private ResponeTo Rep = new ResponeTo();
        private dynamic Reqparams = new DynamicObj();
        private readonly ILogger<ValuesController> _logger;

        public ValuesController(ICapPublisher capPublish, ILogger<ValuesController> logger)
        {
            _IbusPublish = capPublish;
            _logger = logger;

        }
        [HttpGet]
        //public async Task<string> Get([FromServices] ICapPublisher capPublish)
        public async Task<string> Get()
        {
            GUID = Guid.NewGuid().ToString("N");            
            string PubName = "services.show.time";
            // 发布队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams=Rep.CreatPub(GUID, "HttpGet");
            _logger.LogInformation($"{JsonConvert.SerializeObject(Reqparams._values)}");
            // 发布业务
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            //返回请求
            var ToClient = await Rep.ResChangeReturn(GUID);
    
            //_logger.LogError($"{JsonConvert.SerializeObject(ToClient)}");
            //LogMongdHelper.InfoLog("RabbMq", "ValuesController", "Get()", ToClient);
            return JsonConvert.SerializeObject(ToClient);
        }
        [HttpGet("GetName")]
        public async Task<string> GetName(string NameStr)
        {
            GUID = Guid.NewGuid().ToString("N");
            string PubName = "Order.Created";
            // 加入队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams = Rep.CreatPub(GUID, $@"{NameStr}");
            // 发布业务        
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            var ToClient = await Rep.ResChangeReturn(GUID);
            //LogMongdHelper.InfoLog("RabbMq", "ValuesController", "GetName(string NameStr)", ToClient,$"{NameStr}");
            int bb = 10;
            int xx = bb / 0;
            return JsonConvert.SerializeObject(ToClient);
        }
        [HttpGet("GetLog")]
        public async Task<string> GetLog(string lastid)
        {
            GUID = Guid.NewGuid().ToString("N");
            string PubName = "Page.Collection";
            // 加入队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams = Rep.CreatPub(GUID, $@"{lastid}");
            // 发布业务        
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            var ToClient = await Rep.ResChangeReturn(GUID);        
            return JsonConvert.SerializeObject(ToClient);
        }
        [HttpGet("GetMap")]
        public async Task<string> GetMap()
        {
            
            GUID = Guid.NewGuid().ToString("N");
            string PubName = "Map.Collection";
            // 加入队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams = Rep.CreatPub(GUID, $@"Map");
            // 发布业务        
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            var ToClient = await Rep.ResChangeReturn(GUID);
            return JsonConvert.SerializeObject(ToClient);
        }
        /*  监听不放在控制器
        [NonAction]
        [CapSubscribe("Order.Created")] //监听Order.Created事件
        public async Task OrderCreatedEventHand(string json)
        {
        
            JObject O = JObject.Parse(json);         
            Rep.ResApiSub((string)O["Guid"], 200, $"{O["Connect"]}",new JArray());
            Console.WriteLine(json);
        }   
        */
       
    } 
}
