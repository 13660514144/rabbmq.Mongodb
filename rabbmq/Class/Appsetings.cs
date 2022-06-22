using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Class
{
    public class Appsetings
    {
        public int QueCash;
        public string LogsPath;
        public string CurrentIp;
        public RabbitMQ RabbitMQCfg;
        public SQL SQLCfg;
        public MongoDbCap MongoDbCapCfg;
        public MongoDbLog MongoDbLogCfg;
        public MongoDbMap MongoDbMapCfg;
        public Appsetings()
        {
            var config = MyServiceProvider.ServiceProvider.GetRequiredService<GetAppsetting>();
            QueCash = Convert.ToInt32(config.ConfigAppset.GetSection("QueCash").Value);
            LogsPath = config.ConfigAppset.GetSection("LogsPath").Value.ToString();
            CurrentIp = config.ConfigAppset.GetSection("CurrentIp").Value.ToString();
            RabbitMQCfg = new RabbitMQ
            { 
                HostName= config.ConfigAppset.GetSection("RabbitMQ").GetSection("HostName").Value.ToString(),
                UserName = config.ConfigAppset.GetSection("RabbitMQ").GetSection("UserName").Value.ToString(),
                Password = config.ConfigAppset.GetSection("RabbitMQ").GetSection("Password").Value.ToString(),
                VirtualHost = config.ConfigAppset.GetSection("RabbitMQ").GetSection("VirtualHost").Value.ToString(),
                Port = Convert.ToInt32(config.ConfigAppset.GetSection("RabbitMQ").GetSection("Port").Value),
                ExchangeName = config.ConfigAppset.GetSection("RabbitMQ").GetSection("ExchangeName").Value.ToString(),
                FailedRetryCount = Convert.ToInt32(config.ConfigAppset.GetSection("RabbitMQ").GetSection("FailedRetryCount").Value),
                FailedRetryInterval = Convert.ToInt32(config.ConfigAppset.GetSection("RabbitMQ").GetSection("FailedRetryInterval").Value),
                SucceedMessageExpiredAfter = Convert.ToInt32(config.ConfigAppset.GetSection("RabbitMQ").GetSection("SucceedMessageExpiredAfter").Value)
            };
            
            SQLCfg = new SQL
            { 
                Mysql_Conn= config.ConfigAppset.GetSection("ConnectionStrings").GetSection("Mysql_Conn").Value,
                Mssql_Conn = config.ConfigAppset.GetSection("ConnectionStrings").GetSection("Mssql_Conn").Value,
                Mogon_Conn= config.ConfigAppset.GetSection("ConnectionStrings").GetSection("Mogon_Conn").Value
            };

            MongoDbCapCfg = new MongoDbCap
            {
                mongodb_Connection= config.ConfigAppset.GetSection("MongoDb").GetSection("mongodb_Connection").Value,
                mongodb_DataBase = config.ConfigAppset.GetSection("MongoDb").GetSection("mongodb_DataBase").Value,
                t_user = config.ConfigAppset.GetSection("MongoDb").GetSection("t_user").Value
            };

            MongoDbLogCfg = new MongoDbLog
            {
                mongodb_Connection = config.ConfigAppset.GetSection("MongoDbLog").GetSection("mongodb_Connection").Value,
                mongodb_DataBase = config.ConfigAppset.GetSection("MongoDbLog").GetSection("mongodb_DataBase").Value,
                t_user = config.ConfigAppset.GetSection("MongoDbLog").GetSection("t_user").Value
            };
            MongoDbMapCfg = new MongoDbMap
            {
                mongodb_Connection = config.ConfigAppset.GetSection("MongoDbMap").GetSection("mongodb_Connection").Value,
                mongodb_DataBase = config.ConfigAppset.GetSection("MongoDbMap").GetSection("mongodb_DataBase").Value,
                t_user = config.ConfigAppset.GetSection("MongoDbMap").GetSection("t_user").Value
            };
        }
        public class MongoHost
        { 
        }
        public class RabbitMQ
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string VirtualHost { get; set; }
            public int Port { get; set; }
            public string ExchangeName { get; set; }
            public int FailedRetryCount { get; set; }
            public int FailedRetryInterval { get; set; }
            public int SucceedMessageExpiredAfter { get; set; }
        }
        public class SQL
        {
            public string Mysql_Conn { get; set; }
            public string Mssql_Conn { get; set; }
            public string Mogon_Conn { get; set; }
        }
        public class MongoDbCap
        {
            public string mongodb_Connection { get; set; }
            public string mongodb_DataBase { get; set; }
            public string t_user { get; set; }
        }
        public class MongoDbLog
        {
            public string mongodb_Connection { get; set; }
            public string mongodb_DataBase { get; set; }
            public string t_user { get; set; }
        }
        public class MongoDbMap
        {
            public string mongodb_Connection { get; set; }
            public string mongodb_DataBase { get; set; }
            public string t_user { get; set; }
        }
    }
}
