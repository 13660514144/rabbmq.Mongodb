using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using HelperTools;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rabbmq.AssemblyRegister;
using rabbmq.Dbclass;


namespace rabbmq.Class
{
    public class ResponeTo
    {
        public RpModels ResponseModel;
        private int CashQue;
        private ICapPublisher Ipublish;
        public ResponeTo()
        {
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            CashQue = Settings.QueCash;
        }
        /// <summary>
        /// 发布队列
        /// </summary>
        /// <param name="GUID"></param>
        public void ResChangePub(string GUID)
        {
            
            switch (CashQue)
            {
                case 1:
                    ResApiPub(GUID);
                    break;
                case 2:
                    ResApiPubMongo(GUID);
                    break;
            }
        }
        /// <summary>
        /// 返回队列
        /// </summary>
        /// <param name="GUID"></param>
        public async Task<RpModels> ResChangeReturn(string GUID)
        {
            RpModels ToClient=new RpModels();
            switch (CashQue)
            {
                case 1:
                    ToClient=await ResApiReturn(GUID);
                    break;
                case 2:
                    ToClient=await ResApiReturnMongo(GUID);
                    break;
            }
            return ToClient;
        }
        /// <summary>
        /// api list queue
        /// </summary>
        /// <param name="Guid"></param>
        public void ResApiPub(string Guid)
        {
            var Api = MyServiceProvider.ServiceProvider.GetRequiredService<ApiQueue>();
            Api.Que.Guid = Guid;
            Api.Que.Connect = string.Empty;
            Api.Que.ExistValue = false;
            Api.Que.Timespan = TimeSpans.Timestamp();
            Api.ListQue.Add(Api.Que);
        }
        /// <summary>
        /// mongo db api queue
        /// </summary>
        /// <param name="Guid"></param>
        public void ResApiPubMongo(string Guid)
        {

            MongoDBBll.SetHost("cap");
            QueueEntity M = new QueueEntity();
            M._id= ObjectId.GenerateNewId();
            M.Guid = Guid;
            M.Connect = string.Empty;
            M.ExistValue = false;
            M.Timespan = TimeSpans.Timestamp();
            bool Flg = MongoDBBll.InsertQUE(M);
        }
        public void ResApisubChange(string Guid, int code, string message, object data,object fieldsinfo=null)
        {
            switch (CashQue)
            {
                case 1:
                    ResApiSub( Guid, code, message, data, fieldsinfo);
                    break;
                case 2:
                    ResApiSubMongoDb(Guid, code, message, data, fieldsinfo);
                    break;
            }
        }
        public dynamic CreatPub(string GUID,object Params=null)
        {
            dynamic Obj = new DynamicObj();
            Obj.Guid = GUID;
            Obj.Connect = Params;
            return Obj;
        }
        public void ResApiSub(string Guid, int code,string message,object data, object fieldinfo = null)
        {
            try
            {
                var Api = MyServiceProvider.ServiceProvider.GetRequiredService<ApiQueue>();                
                var list = Api.ListQue.Where(c => c.Guid.Equals(Guid)).FirstOrDefault();
                if (list !=null && !list.ExistValue)
                {
                    ResponseModel = new RpModels();
                    ResponseModel.code = code;
                    ResponseModel.message = message;
                    ResponseModel.data = data;
                    ResponseModel.fieldsinfo = fieldinfo;
                    list.Connect = ResponseModel;
                    list.ExistValue = true;
                }                
            }
            catch (Exception ex)
            { 
            }
        }
        /// <summary>
        /// mogodb
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        public void ResApiSubMongoDb(string Guid, int code, string message, object data,object fieldinfo=null)
        {
            try
            {
                ResponseModel = new RpModels();
                ResponseModel.code = code;
                ResponseModel.message = message;
                ResponseModel.data = data;
                ResponseModel.fieldsinfo = fieldinfo;
                MongoDBBll.SetHost("cap");
                QueueEntity M = new QueueEntity();
                M._id = ObjectId.GenerateNewId();
                M.Guid = Guid;
                M.Connect = ResponseModel;
                M.ExistValue = true;
                bool Flg=MongoDBBll.UpdateQUE(M,Guid);
            }
            catch (Exception ex)
            {
            }
        }
        public async  Task<RpModels> ResApiReturn(string Guid)
        {
            ResponseModel = new RpModels();
            bool Flg = false;
            try
            {
                var Api = MyServiceProvider.ServiceProvider.GetRequiredService<ApiQueue>();
                for (int x = 0; x < 6000; x++)
                {
                    var list = Api.ListQue.Where(c => c.Guid.Equals(Guid)).FirstOrDefault();
                    if (list != null)
                    {
                        if (list.ExistValue)
                        {                            
                            ResponseModel = (RpModels)list.Connect;
                            Flg = true;                            
                            break;
                        }
                    }
                    await Task.Delay(10);
                }
                Api.ListQue.RemoveAll(x => x.Guid == Guid);
                if (!Flg)
                {
                    ResponseModel.code = 500;
                    ResponseModel.message = $"找不到主键GUID";
                    ResponseModel.data = new JArray();                    
                }
            }
            catch (Exception ex)
            {
                ResponseModel.code = 500;
                ResponseModel.message = $"error:{ex.Message}==>{ex.Source}==>{ex.StackTrace}";
                ResponseModel.data = new JArray();
                LogMongdHelper.ErrorLog(ex,"RabbMq", "ResponeTo", "ResApiReturn", Guid);
            }
    
            return ResponseModel;
        }
        /// <summary>
        /// mogo db
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public async Task<RpModels> ResApiReturnMongo(string Guid)
        {
            ResponseModel = new RpModels();
            QueueEntity O = new QueueEntity();
            bool Flg = false;
            try
            {
                MongoDBBll.SetHost("cap");               
                for (int x = 0; x < 6000; x++)
                {
                    List<QueueEntity> Json = MongoDBBll.SELECTQUE(Guid);                    
                    if (Json.Count>0)
                    {
                        O = Json[0];
                        if (O.ExistValue)
                        {
                            ResponseModel = (RpModels)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(O.Connect), typeof(RpModels));
                            Flg = true;
                            bool flg = MongoDBBll.DeleteQUE(O.Guid);
                            break;
                        }
                    }
                    await Task.Delay(10);
                }
                
                if (!Flg)
                {
                    ResponseModel.code = 500;
                    ResponseModel.message = $"找不到主键GUID";
                    ResponseModel.data = new JArray();                    
                }
            }
            catch (Exception ex)
            {
                ResponseModel.code = 500;
                ResponseModel.message = $"error:{ex.Message}==>{ex.Source}==>{ex.StackTrace}";
                ResponseModel.data = new JArray();
                LogMongdHelper.ErrorLog(ex, "RabbMq", "ResponeTo", "ResApiReturnMongo", Guid);
            }

            return ResponseModel;
        }
    }
    public class RpModels
    {
        public int code { get; set; } = 200;
        public string message { get; set; } = string.Empty;
        public object data { get; set; }
        public object fieldsinfo { get; set; }
    }
}
