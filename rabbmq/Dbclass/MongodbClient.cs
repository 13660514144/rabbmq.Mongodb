using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
namespace rabbmq.Dbclass
{
    //链接数据库
    //static MongodbHost Host = new MongodbHost()
    //{
    //    Connection = ConfigurationManager.AppSettings["mongodb_Connection"],//链接字符串
    ///    DataBase = ConfigurationManager.AppSettings["mongodb_DataBase"],//库名
    //    Table = "t_user"//表名
    //};
    public class MongodbClient<T> where T : class
    {
        #region +MongodbInfoClient 获取mongodb实例
        /// <summary>
        /// 获取mongodb实例
        /// </summary>
        /// <param name="host">连接字符串，库，表</param>
        /// <returns></returns>
        public static IMongoCollection<T> MongodbInfoClient(MongodbHost host)
        {
            MongoClient client = new MongoClient(host.Connection);
            var dataBase = client.GetDatabase(host.DataBase);
            return dataBase.GetCollection<T>(host.Table);
        }
        #endregion        
    }
    public class MongodbHost
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connection { get; set; }
        /// <summary>
        /// 库
        /// </summary>
        public string DataBase { get; set; }
        /// <summary>
        /// 表
        /// </summary>
        public string Table { get; set; }

    }
}
