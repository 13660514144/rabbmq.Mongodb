using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MongodbLibrary
{
    public class MongoDbDal<T> where T : class, new()
    {
        public static string Tb = string.Empty;
        //全局变量
        public static MongodbHost Host;
        public static void SetHost(string CollectionName)
        {
            var Settings = MongoServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string Dburl = string.Empty;
            string Dbbase = string.Empty;

            switch (CollectionName.ToUpper())
            {
                case "CAP":
                    Dburl = Settings.MongoDbCapCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbCapCfg.mongodb_DataBase;
                    break;
                case "LOG":
                    Dburl = Settings.MongoDbLogCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbLogCfg.mongodb_DataBase;
                    break;
                case "MAP":
                    Dburl = Settings.MongoDbMapCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbMapCfg.mongodb_DataBase;
                    break;
            }
            Host = new MongodbHost()
            {
                Connection = Dburl,//链接字符串
                DataBase = Dbbase,//库名
                Table = Tb //表名
            };
        }
        public static void GuestSetHost(string Url,string Db,string Collection)
        {
            Host = new MongodbHost()
            {
                Connection = Url,//链接字符串
                DataBase = Db,//库名
                Table = Collection //表名
            };
        }
        
        #region Insert record
        /// <summary>
        /// 同步单表记录写入 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool InsertOne(T t)
        {
            int i = TMongodbHelper<T>.Add(Host, t);//成功返回1，失败返回0
            return i > 0;
        }
        /// <summary>
        ///  同步 写入批量数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool InsertMany(List<T> list)
        {
            int i = TMongodbHelper<T>.InsertMany(Host, list);//成功返回1，失败返回0
            return i > 0;
        }
        
        /// <summary>
        /// 异步  单表记录写入 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static async Task<bool> InsertOneAsync(T t)
        {
            int i = await TMongodbHelper<T>.AddAsync(Host, t);//成功返回1，失败返回0
            return i > 0;
        }
        /// <summary>
        ///  异步 写入批量数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static async Task<bool> InsertManyAsync(List<T> list)
        {
            int i = await TMongodbHelper<T>.InsertManyAsync(Host, list);//成功返回1，失败返回0
            return i > 0;
        }
        #endregion

        #region Update Data
        /// <summary>
        /// 同步 更新记录 主键
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Update(T t, string id)
        {
            var u = TMongodbHelper<T>.Update(Host, t, id);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
        /// <summary>
        /// 同步 更新记录 关键字段
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool UpdateField(T t, Dictionary<string,object> Fields)
        {
            var u = TMongodbHelper<T>.UpdateField(Host, t, Fields);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
        /// <summary>
        /// 异步 更新记录 主键
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateAsync(T t, string id)
        {
            var u = await TMongodbHelper<T>.UpdateAsync(Host, t, id);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
        /// <summary>
        /// 异步 更新记录 关键字段
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async  Task<bool> UpdateFieldAsync(T t, Dictionary<string, object> Fields)
        {
            var u = await TMongodbHelper<T>.UpdateFieldAsync(Host, t, Fields);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
        #endregion

        #region delete recored
        /// <summary>
        /// 同步 异步  主键ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Delete(string id)
        {
            var d = TMongodbHelper<T>.Delete(Host, id);
            return d.DeletedCount > 0;
        }
        public static async Task<bool> DeleteAsync(string id)
        {
            var d =await TMongodbHelper<T>.DeleteAsync(Host, id);
            return d.DeletedCount > 0;
        }
        /// <summary>
        /// 同步 异步  关键字段ID删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteField(Dictionary<string,object> Fields)
        {
     
            var d = TMongodbHelper<T>.DeleteField(Host, Fields);
            return d.DeletedCount > 0;
        }
        public static async Task<bool> DeleteFieldAsync(Dictionary<string, object> Fields)
        {

            var d = await TMongodbHelper<T>.DeleteFieldAsync(Host, Fields);
            return d.DeletedCount > 0;
        }
        #endregion

        #region get on record
        /// <summary>
        /// 绑定主键ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T BindById(string id)
        {
            var res = TMongodbHelper<T>.FindOne(Host, id);            
            return res;
        }
        public static async Task<T> BindByIdAsync(string id)
        {
            var res = await TMongodbHelper<T>.FindOneAsync(Host, id);
            return res;
        }
        /// <summary>
        /// 绑定fields
        /// </summary>
        /// <param name="Fields"></param>
        /// <returns></returns>
        public static T BindByFields(Dictionary<string,object> Fields)
        {
            var res = TMongodbHelper<T>.FindOneFields(Host, Fields);
            return res;
        }
        public static async Task<T> BindByFieldsAsync(Dictionary<string, object> Fields)
        {
            var res = await  TMongodbHelper<T>.FindOneFieldsAsync(Host, Fields);
            return res;
        }
        public static List<T> BindListFields(Dictionary<string, object> Fields)
        {
            var res = TMongodbHelper<T>.FindListFields(Host, Fields);
            return res;
        }
        public static async Task<List<T>> BindListFieldsAsync(Dictionary<string, object> Fields)
        {
            var res =await TMongodbHelper<T>.FindListFieldsAsync(Host, Fields);
            return res;
        }
        #endregion

        #region guest list data search
        public static async Task<SysConfigEnity> GetDefSysCconfig(string Id="")
        {
            var Settings = MongoServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string IdCode = (string.IsNullOrEmpty(Id) == true) ? "61bbec60b892f65e6042d837" : Id;
            string MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
            GuestSetHost(MongoUrl, "ownerconfig", "sysconfig");
            var res = await TMongodbHelper<SysConfigEnity>.FindOneAsync(Host, IdCode);
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Paras"></param>
        /// <returns></returns>
        public static async Task<List<T>> SearchListAsync(SearchEntity Paras)
        {
            #region
            /* FieldType N=字符型 I=数字型 D=日期型  IdCode 对应 db tb 
            {
                "IdCode":"IdCode",
                "Role":"Role",
                "DelFlg":1,
                "GroupFlg":"GroupFlg",
                "LastId","",
                "PageNextOrPre":"",
                "SortField":"",
                "WhereCollection":[
                    {
                        "FieldName":"FieldName",
                        "FieldType":"FieldType",
                        "FieldValue":"FieldValue"
                    },
                    {
                        "FieldName":"FieldName",
                        "FieldType":"FieldType",
                        "FieldValue":"FieldValue"
                    }
                ]
            }
            */


            //JArray Arr = (JArray)Paras["WhereCollection"];
            var list = new List<FilterDefinition<T>>();
            for (int x = 0; x < Paras.WhereCollection.Count; x++)
            {
                var oo = Paras.WhereCollection[x];
                //JObject F = (JObject)Paras.WhereCollection[x];
                switch (oo.FieldType)
                {
                    case "N":
                        list.Add(Builders<T>.Filter.Eq(oo.FieldName, oo.FieldValue));
                        break;
                    case "I":
                        list.Add(Builders<T>.Filter.Eq(oo.FieldName, oo.FieldValue));
                        break;
                    case "D":
                        string[] Da = oo.FieldValue.ToString().Split('#');
                        list.Add(Builders<T>.Filter.Gte(oo.FieldName, Convert.ToDateTime($"{Da[0]} 00:00:00")));
                        list.Add(Builders<T>.Filter.Lte(oo.FieldName, Convert.ToDateTime($"{Da[1]} 23:59:59")));
                        break;
                }                
            }
            if (Paras.PageNextOrPre.ToLower() == "next")
            {
                if (Paras.LastId.Length == 24)
                {
                    list.Add(Builders<T>.Filter.Lt("_id", new ObjectId(Paras.LastId)));
                }
                else 
                {
                    list.Add(Builders<T>.Filter.Lt("_id", Paras.LastId));
                }
            }
            else if (Paras.PageNextOrPre.ToLower() == "pre")
            {
                if (Paras.LastId.Length == 24)
                {
                    list.Add(Builders<T>.Filter.Gt("_id", new ObjectId(Paras.LastId)));
                }
                else 
                {
                    list.Add(Builders<T>.Filter.Gt("_id", Paras.LastId));
                }
            }
            var filter =list.Count>0 ? Builders<T>.Filter.And(list): Builders<T>.Filter.Empty;
            var listsort = new List<SortDefinition<T>>();
            if (Paras.SortField==null)
            {
                listsort.Add(Builders<T>.Sort.Descending("_id"));
            }
            else  
            {
                JArray sArr =(JArray)Paras.SortField;
                for (int x = 0; x < sArr.Count; x++)
                {
                    JObject Sobj = (JObject)sArr[x];
                    listsort.Add(
                        (Paras.PageNextOrPre.ToLower() == "next" || Paras.PageNextOrPre.ToLower() == "first")
                        ? Builders<T>.Sort.Descending((string)Sobj["Field"])
                        : Builders<T>.Sort.Ascending((string)Sobj["Field"])
                        ) ;
                }
            }
            var sort = Builders<T>.Sort.Combine(listsort);
            var ress = await  TMongodbHelper<T>.SearchListAsync(Host, filter,null,sort,Paras.PageRows,true);
            return ress;
            #endregion
        }
        #endregion
    }
}
