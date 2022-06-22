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
    public class TestSubscriber
    {
        
        public interface ISubscriberService
        {
            public void CheckReceivedMessage(string Json);
            public Task OrderCreatedEventHand(string json);
            public Task PageEventHand(string json);
            public Task MapEventHand(string json);
        }

        public class SubscriberService : ISubscriberService, ICapSubscribe
        {
            private ICapPublisher Ipublish;
            private ResponeTo Rep = new ResponeTo();
            public SubscriberService(ICapPublisher _ICapPublisher)
            {
                Ipublish = _ICapPublisher;
            }
            [CapSubscribe("services.show.time")]
            public void CheckReceivedMessage(string Json)
            {
                JObject O = JObject.Parse(Json);
                Rep.ResApisubChange((string)O["Guid"], 200, $"time:{O["Connect"]}", new JArray());
                Console.WriteLine($"事件订阅成功=>{Json}");
                //Ipublish.PublishAsync<string>("show.time.return", $"收到消息:{time}");
            }
            [CapSubscribe("Order.Created")] 
            public async Task OrderCreatedEventHand(string json)
            {
                JObject O = JObject.Parse(json);
                Rep.ResApisubChange((string)O["Guid"], 200, $"{O["Connect"]}", new JArray());
                Console.WriteLine(json);
            }
            [CapSubscribe("Page.Collection")] 
            public async Task PageEventHand(string json)
            {               
                JObject O = JObject.Parse(json);
                MongoDBBll.SetHost("log");
                var Arr=MongoDBBll.SelectLog(string.Empty);
                Rep.ResApisubChange((string)O["Guid"], 200, $"{O["Connect"]}", Arr);               
            }
            [CapSubscribe("Map.Collection")] 
            public async Task MapEventHand(string json)
            {
                JObject O = JObject.Parse(json);

                //MongoDBBll.SetHost("map");
                //Console.WriteLine($"select map start =>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                //var s = MongoDBBll.SelectMap();
                //Console.WriteLine($"select map end =>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                
                //Rep.ResApisubChange((string)O["Guid"], 200, $"{O["Connect"]}", new JArray());
                
                //MongoDbDal<MapEnity>.Tb = "map";
                //MongoDbDal<MapEnity>.SetHost("map");
                /*Map m = new Map
                { 
                    pop=17338 
                };*/
                Dictionary<string, object> d = new Dictionary<string, object>
                {
                    { "city","BARRE"}
                };
                JObject oo = new JObject
                {
                    new JProperty("IdCode","61bbec09b892f65e6042d836"),
                    new JProperty("Role","Role"),
                    new JProperty("DelFlg",1),
                    new JProperty("GroupFlg","GroupFlg"),
                    new JProperty("LastId",string.Empty),
                    new JProperty("PageNextOrPre",string.Empty),
                    new JProperty("SortField",new JArray{ 
                        new JObject{new JProperty("Field","_id"),new JProperty("Sort", "DSC") },
                        //new JObject{new JProperty("Field","city"),new JProperty("Sort", "ASC") }                        
                    }),
                    new JProperty("WhereCollection",new JArray {
                        new JObject{
                        new JProperty("FieldName","state"),
                        new JProperty("FieldType","N"),
                        new JProperty("FieldValue","MA")
                            }
                        }
                    )                   
                };
                SearchEntity  enity = (SearchEntity)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(oo), typeof(SearchEntity));
                //var ss=await MongoDbDal<MapEnity>.BindListFieldsAsync(d);
                //var ss = await MongoDbDal<SysConfigEnity>.GetDefSysCconfig();
                //Console.WriteLine(JsonConvert.SerializeObject(ss));
                /*61d3beeb71843201341c72f9  1.15.96.81*/
                //dynamic obj = GetEntity.GetModels<Object>(ss.NameSpace, ss.ClassName);61bbec09b892f65e6042d836
                var s1 = await MongoDbDal<dynamic>.GetDefSysCconfig("61d3beeb71843201341c72f9");
                Console.WriteLine(JsonConvert.SerializeObject(s1));
                var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
                string MongoUrl = $"mongodb://{s1.Account}:{s1.Password}@{Settings.SQLCfg.Mogon_Conn}/{s1.Db}";
                MongoDbDal<dynamic>.GuestSetHost(MongoUrl, $"{s1.Db}", $"{s1.Tb}");
                //var ssS = await MongoDbDal<dynamic>.BindListFieldsAsync(d);
                var ssS = await MongoDbDal<dynamic>.SearchListAsync(enity);
                Console.WriteLine(JsonConvert.SerializeObject(ssS));
                Rep.ResApisubChange((string)O["Guid"], 200, $"{O["Connect"]}", ssS);
                //Console.WriteLine($" map INs start =>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                /*await Task.Run(()=> {
                    if (s.Count >0)
                    {
                        
                        int len = s.Count-1;
                        int kk = Convert.ToInt32(s[len]._id);
                        List<Map> map = new List<Map>();
                        
                        for (int x = 0; x <len; x++)
                        {
                            Map maps = new Map();
                            maps._id = (kk+x+1).ToString().PadLeft(11, '0');
                            maps.city = s[x].city;
                            maps.pop = s[x].pop;
                            maps.state = s[x].state;
                            maps.loc = s[x].loc;
                            map.Add(maps);
                            //bool ff = MongoDBBll.InsertMap(maps);
                        }
                        Console.WriteLine($"select map PACKage OVER {s.Count}=>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                        MongoDBBll.SetHost("map");
                        bool ff= MongoDBBll.InsertMaplist(map);
                        Console.WriteLine($@" map INs OVER  =>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}");
                    }
                });*/
            }
        }
    }
}
