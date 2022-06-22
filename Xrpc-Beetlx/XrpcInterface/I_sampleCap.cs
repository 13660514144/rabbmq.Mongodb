using DotNetCore.CAP;
using EventNext;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MongodbLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Xrpc_Beetlx.Models.XrpcReturn;

namespace Xrpc_Beetlx
{
    [Service(typeof(IsampleCap))]
    public class I_sampleCap : IsampleCap
    {
        public I_sampleCap()
        {

        }
        public async Task<string> GetPage(string Obj)
        {
            Console.WriteLine(Obj);
            var ToClient = await ListDataEventHand($@"{Obj}");
            return JsonConvert.SerializeObject(ToClient);

        }
        public Task<string> Hello(string name)
        {
            return $"hello {name} {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}".ToTask();
        }
        public async Task<RpModels> ListDataEventHand(string json)
        {
            JObject O = JObject.Parse(json);
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

            var BaseDb = await MongoDbDal<dynamic>.GetDefSysCconfig($"{O["IdCode"]}");

            var Settings = MongoServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string MongoUrl = $"mongodb://{BaseDb.Account}:{BaseDb.Password}@{Settings.SQLCfg.Mogon_Conn}/{BaseDb.Db}";
            MongoDbDal<dynamic>.GuestSetHost(MongoUrl, $"{BaseDb.Db}", $"{BaseDb.Tb}");
            var Result = await MongoDbDal<dynamic>.SearchListAsync(enity);
            //集合结构信息
            MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
            MongoDbDal<dynamic>.GuestSetHost(MongoUrl, "ownerconfig", "tbstruct");
            Dictionary<string, object> D = new Dictionary<string, object>
                {
                    {"configid",$"{O["IdCode"]}"}
                };
            var CollectionStruct = await MongoDbDal<dynamic>.BindByFieldsAsync(D);
            RpModels ResponseModel = new RpModels();
            ResponseModel.code = 200;
            ResponseModel.message = "查询成功";
            ResponseModel.data = Result;
            ResponseModel.fieldsinfo = CollectionStruct;
            return ResponseModel;
        }
    }
}
