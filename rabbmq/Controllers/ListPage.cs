using DotNetCore.CAP;
using HelperTools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using rabbmq.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListPageController : ControllerBase
    {
        private string GUID;
        private ICapPublisher _IbusPublish;
        private ResponeTo Rep = new ResponeTo();
        private dynamic Reqparams = new DynamicObj();
        private readonly ILogger<ListPageController> _logger;
        public ListPageController(ICapPublisher capPublish, ILogger<ListPageController> logger)
        {
            _IbusPublish = capPublish;
            _logger = logger;

        }
        [HttpPost("GetPage")]
        public async Task<string> GetPage([FromBody] dynamic Obj)
        {
            GUID = Guid.NewGuid().ToString("N");
            string PubName = "ListPage.Collection";
            // 加入队列
            Rep.ResChangePub(GUID);
            //创建发布参数
            Reqparams = Rep.CreatPub(GUID, $"{Obj}");
            // 发布业务        
            await _IbusPublish.PublishAsync<string>(PubName, JsonConvert.SerializeObject(Reqparams._values)); //发布Order.Created事件
            var ToClient = await Rep.ResChangeReturn(GUID);
            //Console.WriteLine(JsonConvert.SerializeObject(ToClient));
            return JsonConvert.SerializeObject(ToClient);
        }
    }
}
