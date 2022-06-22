using DotNetCore.CAP;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rabbmq.AssemblyRegister;
using rabbmq.Dbclass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Class
{
    public class ListPaging
    {
        public interface ISubscriberListService
        {            
            public Task ListDataEventHand(string json);
        }
        public class SubscriberListService : ISubscriberListService, ICapSubscribe
        {
            private ICapPublisher Ipublish;
            private ResponeTo Rep = new ResponeTo();
            public SubscriberListService(ICapPublisher _ICapPublisher)
            {
                Ipublish = _ICapPublisher;
            }

            [CapSubscribe("ListPage.Collection")]
            public async Task ListDataEventHand(string json)
            {
                JObject Obj = JObject.Parse(json);
                JObject O =(JObject)JsonConvert.DeserializeObject(Obj["Connect"].ToString());
                SearchEntity enity = new SearchEntity();
                enity.IdCode = $@"{O["IdCode"]}";
                enity.Role = $@"{O["Role"]}";
                enity.DelFlg = (int)O["DelFlg"];
                enity.GroupFlg = $@"{O["GroupFlg"]}";
                enity.LastId = $@"{O["LastId"]}";
                enity.PageNextOrPre = $@"{O["PageNextOrPre"]}";
                enity.PageRows = (int)O["rows"];
                enity.SortField = new JArray{
                        new JObject{new JProperty("Field","_id"),new JProperty("Sort", "DSC") } };
                JArray Arr = (JArray)O["WhereCollection"];
                for (int x = 0; x < Arr.Count; x++)
                {
                    SearchModels So = (SearchModels)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(Arr[x]), typeof(SearchModels));
                    enity.WhereCollection.Add(So);
                }
                //SearchEntity enity = (SearchEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(oo), typeof(SearchEntity));

                var BaseDb = await MongoDbDal<dynamic>.GetDefSysCconfig($"{O["IdCode"]}");
    
                var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
                string MongoUrl = $"mongodb://{BaseDb.Account}:{BaseDb.Password}@{Settings.SQLCfg.Mogon_Conn}/{BaseDb.Db}";
                MongoDbDal<dynamic>.GuestSetHost(MongoUrl, $"{BaseDb.Db}", $"{BaseDb.Tb}");                
                var Result = await MongoDbDal<dynamic>.SearchListAsync(enity);

                //集合结构信息  根据数据库IDCODE 查表结构
                MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
                MongoDbDal<dynamic>.GuestSetHost(MongoUrl, "ownerconfig", "tbstruct");
                Dictionary<string, object> D = new Dictionary<string, object>
                {
                    {"configid",$"{O["IdCode"]}"}
                };
                var CollectionStruct = await MongoDbDal<dynamic>.BindByFieldsAsync(D);
                Rep.ResApisubChange((string)Obj["Guid"], 200, $"查询成功", Result, CollectionStruct);                
            }
        }
    }
}
