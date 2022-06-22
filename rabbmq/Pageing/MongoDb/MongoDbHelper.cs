using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using rabbmq.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace rabbmq.Pageing.MongoDb
{
    public class MongoDbHelper<T> where T : class, new()
    {
        private IMongoDatabase db = null;
        private IMongoCollection<T> collection = null;
        public MongoDbHelper()
        {
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();   
            this.db = Db.GetDb(Settings.MongoDbLogCfg.mongodb_Connection, Settings.MongoDbLogCfg.mongodb_DataBase);
            this.collection = db.GetCollection<T>(Settings.MongoDbLogCfg.t_user);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PagingModel<T> GetPagingData(FilterDefinition<T> filter, 
            SortDefinition<T> sort, int pageIndex=0, int pageSize=20,string LastId="")
        {
            var list = this.collection
                .Find(filter)
                .Sort(sort)
                //.Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToList();

            var pagingModel = new PagingModel<T>();
            if (list != null)
            {
                var count = list.Count;
                pagingModel.Items = list;
                JObject O = JObject.Parse(list[list.Count-1].ToString());
                pagingModel.PageIndex = pageIndex;
                pagingModel.PageSize = pageSize;
                pagingModel.TotalRecords = Convert.ToInt32(count);
                pagingModel.TotalPages = (int)Math.Ceiling((double)count / (double)pageSize);
                pagingModel.PageLastId = (String)O["_id"];
            }
            return pagingModel;
        }
        /*public PagingModel<T> CreatSearch(string LastId)
        {
            FilterDefinition<LogEntity> filter = Builders<LogEntity>.Filter.Empty;
            filter = Builders<LogEntity>.Filter.Gt("_id", LastId);
            var sort = Builders<LogEntity>.Sort.Descending(c => c._id);
            var pagingModel = GetPagingData(filter,sort,0,20, LastId);
            return pagingModel;
        }*/
    }
}
