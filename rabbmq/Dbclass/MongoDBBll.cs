
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using rabbmq.AssemblyRegister;
using rabbmq.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace rabbmq.Dbclass
{
    public class MongoDBBll
    {        
        public static string Tb = string.Empty;
        //全局变量
        public static MongodbHost Host ;
    
        public static void SetHost(string CollectionName)
        {
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string Dburl =string.Empty;
            string Dbbase = string.Empty;
            
            switch (CollectionName.ToUpper())
            {
                case "CAP":
                    Dburl = Settings.MongoDbCapCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbCapCfg.mongodb_DataBase;
                    Tb = Settings.MongoDbCapCfg.t_user;
                    break;
                case "LOG":
                    Dburl = Settings.MongoDbLogCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbLogCfg.mongodb_DataBase;
                    Tb = Settings.MongoDbLogCfg.t_user;
                    break;
                case "MAP":
                    Dburl = Settings.MongoDbMapCfg.mongodb_Connection;
                    Dbbase = Settings.MongoDbMapCfg.mongodb_DataBase;
                    Tb = Settings.MongoDbMapCfg.t_user;
                    break;
            }
            Host = new MongodbHost()
            {
                Connection = Dburl,//链接字符串
                DataBase = Dbbase,//库名
                Table = Tb //表名
            };
        }
       
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        public static bool InsertLog(object err)
        {
            int i = TMongodbHelper<object>.Add(Host, err);//成功返回1，失败返回0
            return i > 0;
        }
        public static bool InsertQUE(QueueEntity que)
        {
            int i = TMongodbHelper<QueueEntity>.Add(Host, que);//成功返回1，失败返回0
            return i > 0;
        }
        public static bool InsertMap(MapEnity Maps)
        {
            int i = TMongodbHelper<MapEnity>.Add(Host, Maps);//成功返回1，失败返回0
            return i > 0;
        }
        public static bool InsertMaplist(List<MapEnity> Maps)
        {
            int i =  TMongodbHelper<MapEnity>.InsertMany(Host, Maps);//成功返回1，失败返回0
            return i > 0;
        }
        //增
        public static bool Insert(UserEntity user)
        {
            int i = TMongodbHelper<UserEntity>.Add(Host, user);//成功返回1，失败返回0
            return i > 0;
        }
        //查
        public static string SELECT(string name)
        {
            //var filter = Builders<UserEntity>.Filter.Empty;//无查询条件用这个

            //根据条件查询集合
            var time = DateTime.Now;
            var list = new List<FilterDefinition<UserEntity>>();
            list.Add(Builders<UserEntity>.Filter.Lt("createdate", time.AddDays(30)));
            list.Add(Builders<UserEntity>.Filter.Gt("createdate", time.AddDays(-30)));

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))//模糊查询
            {
                string p = name == null ? $".*{Regex.Escape("")}.*" : $".*{Regex.Escape(name)}.*";
                list.Add(Builders<UserEntity>.Filter.Regex("name", new BsonRegularExpression(new Regex(p, RegexOptions.IgnoreCase))));
            }

            var filter = Builders<UserEntity>.Filter.And(list);
            //2.查询字段
            var field = new[] { "_id", "name", "password", "age", "phone" };//_id是mongo自带的主键  , "createdate" 
            //3.排序字段
            var sort = Builders<UserEntity>.Sort.Descending("createdate");

            var res = TMongodbHelper<UserEntity>.FindList(Host, filter, field, sort);
            string json = JsonConvert.SerializeObject(res);
            return json;
        }
        public static List<QueueEntity> SELECTQUE(string Guid)
        {
            //var filter = Builders<UserEntity>.Filter.Empty;//无查询条件用这个

            //根据条件查询集合
            var time = DateTime.Now;
            var list = new List<FilterDefinition<QueueEntity>>();
            //list.Add(Builders<QueueEntity>.Filter.Lt("createdate", time.AddDays(30)));
            //list.Add(Builders<QueueEntity>.Filter.Gt("createdate", time.AddDays(-30)));
            /*
            if (!string.IsNullOrEmpty(Guid) && !string.IsNullOrWhiteSpace(Guid))//模糊查询
            {
                string p = Guid == null ? $".*{Regex.Escape("")}.*" : $".*{Regex.Escape(Guid)}.*";
                list.Add(Builders<QueueEntity>.Filter.Regex("Guid", new BsonRegularExpression(new Regex(p, RegexOptions.IgnoreCase))));
            }
            */
            //var filter = Builders<QueueEntity>.Filter.And(list);
            var filter = Builders<QueueEntity>.Filter.Eq("Guid", Guid);            
            //2.查询字段
            var field = new[] { "_id", "Guid", "Connect", "ExistValue" };
            //3.排序字段
            //var sort = Builders<QueueEntity>.Sort.Descending("createdate");

            List<QueueEntity> res = TMongodbHelper<QueueEntity>.FindList(Host, filter, field);
      
            return res;
        }
        /// <summary>
        /// 日志分页查询
        /// </summary>
        /// <param name="LastId"></param>
        /// <returns></returns>
        public static List<LogEntity> SelectLog(string LastId)
        {            
            var builder = Builders<LogEntity>.Filter;
            var filter = Builders<LogEntity>.Filter.Empty;

            //2.查询字段
            var field = new[] { "_id","Ip", "LogTime", "LogType", "LogMessage" };
            //3.排序字段
            var sort = Builders<LogEntity>.Sort.Descending("_id");
            
            var res = TMongodbHelper<LogEntity>.FindListLog(Host, filter, field, sort);
            
            return res;
        }
        public static List<MapEnity> SelectMap()
        {
            var list = new List<FilterDefinition<MapEnity>>();
            var filter = Builders<MapEnity>.Filter.Empty;
            //var filter = Builders<Map>.Filter.Empty;
            //2.查询字段
            var field = new[] {"city", "loc", "pop","state" };
            //3.排序字段
            var sort = Builders<MapEnity>.Sort.Ascending("_id");
            
            var res = TMongodbHelper<MapEnity>.FindListLog(Host, filter, field,sort,20,false);
            
           
            return res;
        }
        //GUID查询
        public static string BindByGuid(string Guid)
        {
            var field = new[] { "_id", "Guid", "Connect", "ExistValue" };
            var res = TMongodbHelper<QueueEntity>.FindOneGuid(Host, Guid, field);
            string json = JsonConvert.SerializeObject(res);
            return json;
        }
        //主键查询
        public static string BindById(string id)
        {
            var res = TMongodbHelper<UserEntity>.FindOne(Host, id);
            string json = JsonConvert.SerializeObject(res);
            return json;
        }
        public static bool DeleteQUE(string Guid)
        {
            var d = TMongodbHelper<QueueEntity>.DeleteQUE(Host, Guid);
            return d.DeletedCount > 0;
        }
        public static bool DeleteLOG(string Time)
        {
            var d = TMongodbHelper<LogEntity>.DeleteLOG(Host, Time);
            return d.DeletedCount > 0;
        }
        //删
        public static bool Delete(string id)
        {
            var d = TMongodbHelper<UserEntity>.Delete(Host, id);
            return d.DeletedCount > 0;
        }
        /// <summary>
        /// 时间戳删除
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool DeleteQUE(long time)
        {
            var d = TMongodbHelper<QueueEntity>.DeleteQUE(Host, time);
            return d.DeletedCount > 0;
        }
        //改
        public static bool Update(UserEntity user, string id)
        {
            var u = TMongodbHelper<UserEntity>.Update(Host, user, id);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
        public static bool UpdateQUE(QueueEntity que, string Guid)
        {
            var u = TMongodbHelper<QueueEntity>.UpdateQUE(Host, que, Guid);//成功返回1，失败返回0
            return u.ModifiedCount > 0;
        }
    }
}
