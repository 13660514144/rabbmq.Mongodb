﻿using MongoDB.Bson;
using MongoDB.Driver;
using rabbmq.Class;
using rabbmq.Pageing.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Dbclass
{
    public static class TMongodbHelper<T> where T : class, new()
    {
        #region +Add 添加一条数据
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static int Add(MongodbHost host, T t)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                client.InsertOne(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region +AddAsync 异步添加一条数据
        /// <summary>
        /// 异步添加一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static async Task<int> AddAsync(MongodbHost host, T t)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                await client.InsertOneAsync(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region +InsertMany 批量插入
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public static int InsertMany(MongodbHost host, List<T> t)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                client.InsertMany(t);
                return 1;
            }
            catch (Exception ex)
            {
                LogMongdHelper.ErrorLog(ex, "RabbMq", "MongodbHelper", "InsertMany", string.Empty);
                return 0;
            }
        }
        #endregion

        #region +InsertManyAsync 异步批量插入
        /// <summary>
        /// 异步批量插入
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">实体集合</param>
        /// <returns></returns>
        public static async Task<int> InsertManyAsync(MongodbHost host, List<T> t)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                await client.InsertManyAsync(t);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        #endregion
        #region +Update 修改一条数据
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">添加的实体</param>
        /// <param name="Guid">GUID</param>
        /// <returns></returns>
        public static UpdateResult UpdateQUE(MongodbHost host, T t, string Guid)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Guid", Guid);
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "_id") continue;//不能修改主键
                    if (item.Name.ToLower() == "timespan") continue;//不能修改事件戳
                    if (item.GetValue(t) == null) continue;//无传入值不更新
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region +Update 修改一条数据
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">添加的实体</param>
        /// <param name="id">主键,_id</param>
        /// <returns></returns>
        public static UpdateResult Update(MongodbHost host, T t, string id)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "_id") continue;//不能修改主键
                    if (item.GetValue(t) == null) continue;//无传入值不更新
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">添加的实体</param>
        /// <param name="Fields">关键字段</param>
        /// <returns></returns>
        public static UpdateResult UpdateField(MongodbHost host, T t,
            Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                var F = Fields.Keys.ToList();
                var K = Fields.Values.ToList();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq($"{F[0]}", $"{K[0]}");
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "_id") continue;//不能修改主键
                    if (item.GetValue(t) == null) continue;//无传入值不更新
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateOne(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateAsync 异步修改一条数据
        /// <summary>
        /// 异步修改一条数据
        /// </summary>
        /// <param name="t">添加的实体</param>
        /// <param name="host">mongodb连接信息</param>
        /// <returns></returns>
        public static async Task<UpdateResult> UpdateAsync(MongodbHost host, T t, string id)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "id") continue;
                    if (item.GetValue(t) == null) continue;//无传入值不更新
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateOneAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 修改一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="t">添加的实体</param>
        /// <param name="Fields">关键字段</param>
        /// <returns></returns>
        public static async Task<UpdateResult> UpdateFieldAsync(MongodbHost host, T t,
            Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                var F = Fields.Keys.ToList();
                var K = Fields.Values.ToList();
                FilterDefinition<T> filter = Builders<T>.Filter.Eq(F[0], K[0]);
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (item.Name.ToLower() == "_id") continue;//不能修改主键
                    if (item.GetValue(t) == null) continue;//无传入值不更新
                    list.Add(Builders<T>.Update.Set(item.Name, item.GetValue(t)));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateOneAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateManay 批量修改数据
        /// <summary>
        /// 批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public static UpdateResult UpdateManay(MongodbHost host, Dictionary<string, string> dic, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return client.UpdateMany(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region +UpdateManayAsync 异步批量修改数据
        /// <summary>
        /// 异步批量修改数据
        /// </summary>
        /// <param name="dic">要修改的字段</param>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">修改条件</param>
        /// <returns></returns>
        public static async Task<UpdateResult> UpdateManayAsync(MongodbHost host, Dictionary<string, string> dic, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                T t = new T();
                //要修改的字段
                var list = new List<UpdateDefinition<T>>();
                foreach (var item in t.GetType().GetProperties())
                {
                    if (!dic.ContainsKey(item.Name)) continue;
                    var value = dic[item.Name];
                    list.Add(Builders<T>.Update.Set(item.Name, value));
                }
                var updatefilter = Builders<T>.Update.Combine(list);
                return await client.UpdateManyAsync(filter, updatefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Delete 删除一条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public static DeleteResult Delete(MongodbHost host, string id)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return client.DeleteOne(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="Fields">Fields</param>
        /// <returns></returns>
        public static DeleteResult DeleteField(MongodbHost host, Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key,item.Value));
                }
                var filter=Builders<T>.Filter.And(list);                
                return client.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static DeleteResult DeleteQUE(MongodbHost host, long time)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Lt("Timespan", time);
                return client.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static DeleteResult DeleteQUE(MongodbHost host, string Guid)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Guid", Guid);
                return client.DeleteOne(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static DeleteResult DeleteLOG(MongodbHost host, string Time)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Lt("LogTime", Time);
                return client.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteAsync 异步删除一条数据
        /// <summary>
        /// 异步删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteAsync(MongodbHost host, string id)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //修改条件
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                return await client.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 删除一条数据 异步
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectId</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteFieldAsync(MongodbHost host, Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                return await client.DeleteOneAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteMany 删除多条数据
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public static DeleteResult DeleteMany(MongodbHost host, Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                return  client.DeleteMany(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region DeleteManyAsync 异步删除多条数据
        /// <summary>
        /// 异步删除多条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteManyAsync(MongodbHost host, Dictionary<string, object> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                return await client.DeleteManyAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion
        #region DeleteManyAsync 异步删除多条数据，按比较条件
        /// <summary>
        /// 异步删除多条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">删除的条件</param>
        /// <returns></returns>
        public static async Task<DeleteResult> DeleteManyFilterAsync(MongodbHost host, List<DelFindUpFilter> Fields)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                for (int x = 0; x < Fields.Count; x++)
                {
                    switch (Fields[x].FieldFiler.ToLower())
                    {
                        case "eq":
                            list.Add(Builders<T>.Filter.Eq(Fields[x].FieldName, Fields[x].FieldValue));
                            break;
                        case "gt":
                            list.Add(Builders<T>.Filter.Gt(Fields[x].FieldName, Fields[x].FieldValue));
                            break;
                        case "lt":
                            list.Add(Builders<T>.Filter.Lt(Fields[x].FieldName, Fields[x].FieldValue));
                            break;
                        case "gte":
                            list.Add(Builders<T>.Filter.Gte(Fields[x].FieldName, Fields[x].FieldValue));
                            break;
                        case "lte":
                            list.Add(Builders<T>.Filter.Lte(Fields[x].FieldName, Fields[x].FieldValue));
                            break;
                    }
                }              
                var filter = Builders<T>.Filter.And(list);
                return await client.DeleteManyAsync(filter);
                /*if (Fields[0].Mothed.ToLower() == "del")
                {
                    return await client.DeleteManyAsync(filter);
                }
                else if (Fields[0].Mothed.ToLower() == "up")
                { 
                }
                else if (Fields[0].Mothed.ToLower() == "find")
                {
                }*/
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Count 根据条件获取总数
        /// <summary>
        /// 根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static long Count(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                return client.Count(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CountAsync 异步根据条件获取总数
        /// <summary>
        /// 异步根据条件获取总数
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">条件</param>
        /// <returns></returns>
        public static async Task<long> CountAsync(MongodbHost host, FilterDefinition<T> filter)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                return await client.CountAsync(filter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOne 根据id查询一条数据
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <param name="field">要查询的字段，不写时查询全部</param>
        /// <returns></returns>
        public static T FindOne(MongodbHost host, string id, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return client.Find(filter).FirstOrDefault<T>();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return client.Find(filter).Project<T>(projection).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public static T FindOneFields(MongodbHost host, 
            Dictionary<string, object> Fields, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);                
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return client.Find(filter).FirstOrDefault<T>();
                }
                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return client.Find(filter).Project<T>(projection).FirstOrDefault<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<T> FindOneFieldsAsync(MongodbHost host,
            Dictionary<string, object> Fields, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await  client.Find(filter).FirstOrDefaultAsync<T>();
                }
                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return await  client.Find(filter).Project<T>(projection).FirstOrDefaultAsync<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static List<T> FindListFields(MongodbHost host,
            Dictionary<string, object> Fields, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return client.Find(filter).ToList();
                }
                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return client.Find(filter).Project<T>(projection).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task<List<T>> FindListFieldsAsync(MongodbHost host,
            Dictionary<string, object> Fields, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                var list = new List<FilterDefinition<T>>();
                foreach (var item in Fields)
                {
                    list.Add(Builders<T>.Filter.Eq(item.Key, item.Value));
                }
                var filter = Builders<T>.Filter.And(list);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await  client.Find(filter).ToListAsync();
                }
                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return await  client.Find(filter).Project<T>(projection).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOne 根据GUid查询一条数据
        /// <summary>
        /// 根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="Guid">objectid</param>
        /// <param name="field">要查询的字段，不写时查询全部</param>
        /// <returns></returns>
        public static T FindOneGuid(MongodbHost host, string Guid, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("Guid", Guid);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return client.Find(filter).FirstOrDefault<T>();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return client.Find(filter).Project<T>(projection).First<T>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindOneAsync 异步根据id查询一条数据
        /// <summary>
        /// 异步根据id查询一条数据
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="id">objectid</param>
        /// <returns></returns>
        public static async Task<T> FindOneAsync(MongodbHost host, string id, string[] field = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", new ObjectId(id));
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    return await client.Find(filter).FirstOrDefaultAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                return await client.Find(filter).Project<T>(projection).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindList 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件(必须的)</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static List<T> FindList(MongodbHost host, 
            FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return client.Find(filter).ToList();
                    //进行排序
                    return client.Find(filter).Sort(sort).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return client.Find(filter).Project<T>(projection).ToList();
                //排序查询
                return client.Find(filter).Sort(sort).Project<T>(projection).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region FindListLog 查询集合
        /// <summary>
        /// 查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件(必须的)</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static List<T> FindListLog(MongodbHost host,
            FilterDefinition<T> filter, string[] field = null, 
            SortDefinition<T> sort = null,int PageSize=20,bool IsPaging=true)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return IsPaging == true 
                            ? client.Find(filter).Limit(PageSize).ToList() :
                            client.Find(filter).ToList();
                    //进行排序
                    return IsPaging == true 
                        ? client.Find(filter).Limit(PageSize).Sort(sort).ToList():
                        client.Find(filter).Sort(sort).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString())) ;               
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return IsPaging == true 
                        ? client.Find(filter).Project<T>(projection).Limit(PageSize).ToList()
                        : client.Find(filter).Project<T>(projection).ToList();
                //排序查询
                return IsPaging == true 
                    ? client.Find(filter).Project<T>(projection).Limit(PageSize).Sort(sort).ToList()
                    : client.Find(filter).Project<T>(projection).Sort(sort).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 公共列表查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="filter"></param>
        /// <param name="field"></param>
        /// <param name="sort"></param>
        /// <param name="PageSize"></param>
        /// <param name="IsPaging"></param>
        /// <returns></returns>
        public static async Task<List<T>> SearchListAsync(MongodbHost host,
            FilterDefinition<T> filter, string[] field = null,
            SortDefinition<T> sort = null, int PageSize = 20, bool IsPaging = true)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return IsPaging == true
                            ? await client.Find(filter).Limit(PageSize).ToListAsync() 
                            : await client.Find(filter).ToListAsync();
                    //进行排序
                    return IsPaging == true
                        ? await  client.Find(filter).Limit(PageSize).Sort(sort).ToListAsync() 
                        : await  client.Find(filter).Sort(sort).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return IsPaging == true
                        ? await  client.Find(filter).Project<T>(projection).Limit(PageSize).ToListAsync()
                        : await  client.Find(filter).Project<T>(projection).ToListAsync();
                //排序查询
                return IsPaging == true
                    ? await  client.Find(filter).Project<T>(projection).Sort(sort).Limit(PageSize).ToListAsync()
                    : await  client.Find(filter).Project<T>(projection).Sort(sort).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListAsync 异步查询集合
        /// <summary>
        /// 异步查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static async Task<List<T>> FindListAsync(MongodbHost host, 
            FilterDefinition<T> filter, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).ToListAsync();
                    return await client.Find(filter).Sort(sort).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();
                if (sort == null) return await client.Find(filter).Project<T>(projection).ToListAsync();
                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPage 分页查询集合
        /// <summary>
        /// 分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="count">总条数</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static List<T> FindListByPage(MongodbHost host, FilterDefinition<T> filter, int pageIndex, int pageSize, out long count, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                count = client.Count(filter);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                    //进行排序
                    return client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

                //排序查询
                return client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region FindListByPageAsync 异步分页查询集合
        /// <summary>
        /// 异步分页查询集合
        /// </summary>
        /// <param name="host">mongodb连接信息</param>
        /// <param name="filter">查询条件</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="field">要查询的字段,不写时查询全部</param>
        /// <param name="sort">要排序的字段</param>
        /// <returns></returns>
        public static async Task<List<T>> FindListByPageAsync(MongodbHost host, FilterDefinition<T> filter, int pageIndex, int pageSize, string[] field = null, SortDefinition<T> sort = null)
        {
            try
            {
                var client = MongodbClient<T>.MongodbInfoClient(host);
                //不指定查询字段
                if (field == null || field.Length == 0)
                {
                    if (sort == null) return await client.Find(filter).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                    //进行排序
                    return await client.Find(filter).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();
                }

                //制定查询字段
                var fieldList = new List<ProjectionDefinition<T>>();
                for (int i = 0; i < field.Length; i++)
                {
                    fieldList.Add(Builders<T>.Projection.Include(field[i].ToString()));
                }
                var projection = Builders<T>.Projection.Combine(fieldList);
                fieldList?.Clear();

                //不排序
                if (sort == null) return await client.Find(filter).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

                //排序查询
                return await client.Find(filter).Sort(sort).Project<T>(projection).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToListAsync();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }
}