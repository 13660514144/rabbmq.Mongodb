using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Pageing.MongoDb
{
    public class Db
    {
        private static IMongoDatabase db = null;

        private static readonly object lockHelper = new object();

        private Db() { }

        public static IMongoDatabase GetDb(string connStr, string dbName)
        {
            if (db == null)
            {
                lock (lockHelper)
                {
                    if (db == null)
                    {
                        var client = new MongoClient(connStr);
                        db = client.GetDatabase(dbName);
                    }
                }
            }
            return db;
        }
    }
}
