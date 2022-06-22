using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongodbLibrary
{
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class UserEntity
    {
        public ObjectId _id { get; set; } //mongodb的主键(类似guid)，如果不需要可以删除此行(但是mongodb会自动加上_id)
        public string name { get; set; }
        public string password { get; set; }
        public int age { get; set; }
        public string phone { get; set; }
        public DateTime createdate { get; set; }
    }
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class QueueEntity
    {
        public ObjectId _id { get; set; }
        public string Guid { get; set; }
        public object Connect { get; set; } = string.Empty;
        public bool ExistValue { get; set; } = false;
        public long Timespan { get; set; }
    }
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class LogEntity
    {
        public ObjectId _id { get; set; } //mongodb的主键(类似guid)，如果不需要可以删除此行(但是mongodb会自动加上_id)
        public string LogProjecdtSrc { get; set; } = string.Empty;
        public string LogApiClassSrc { get; set; } = string.Empty;
        public string LogMothedSrc { get; set; } = string.Empty;
        public string LogServerOrClient { get; set; } = "Server";
        public string Ip { get; set; } = string.Empty;
        public DateTime LogTime { get; set; } = DateTime.Now;
        public string LogType { get; set; }
        public object LogMessage { get; set; }
        public object SrcData { get; set; }
    }
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class MapEnity
    {
        //public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public string _id { get; set; }
        public string city { get; set; }
        public object loc { get; set; }
        public long pop { get; set; }
        public string state { get; set; }
    }
    /// <summary>
    /// 配置表
    /// </summary>
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class SysConfigEnity
    {
        public ObjectId _id { get; set; }
        public string Db { get; set; }
        public string Tb { get; set; }
        public string Account { get; set; } 
        public string Password { get; set; }
        public string NameSpace { get;set;}
        public string ClassName { get; set; }
    }

    public class SearchModels
    { 
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public object FieldValue { get; set; }
    }
    public class SearchEntity
    {
        public string IdCode { get; set; }
        public string Role { get; set; }
        public int DelFlg { get; set; } = 1;
        public string GroupFlg { get; set; }
        public string LastId { get; set; }
        public string PageNextOrPre { get; set; }
        public object SortField { get; set; }
        public int PageRows { get; set; } = 20;
        public List<SearchModels> WhereCollection { get; set; } = new List<SearchModels>();
    }
    /// <summary>
    /// websocket 
    /// </summary>
    [BsonIgnoreExtraElements]//忽略mongodb内部自动产生的一些字段
    public class WebSocketNetty
    {
        public ObjectId _id { get; set; }
        public string ConnectId { get; set; }
        public string Ip { get; set; }
        public string Port { get; set; }
        public string Type { get; set; }//app or webview or other
        public long Timespan { get; set; }//最后一次心跳时间

    }
}
