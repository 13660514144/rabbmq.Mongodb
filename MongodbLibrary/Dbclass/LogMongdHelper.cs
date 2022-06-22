using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
namespace MongodbLibrary
{
    public class LogMongdHelper
    {
        public static void ErrorLog(Exception ex,
            string LogProjecdtSrc,
            string LogApiClassSrc,string LogMothedSrc,string SrcData=null)
        {

            try
            {
                var Err = new
                {
                    Message=ex.Message,
                    Source=ex.Source,
                    StackTrace=ex.StackTrace
                };
                var Settings = MongoServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
                LogEntity Errenity = new LogEntity();
                Errenity._id = ObjectId.GenerateNewId();
                Errenity.LogProjecdtSrc = LogProjecdtSrc;
                Errenity.LogApiClassSrc = LogApiClassSrc;
                Errenity.LogMothedSrc = LogMothedSrc;
                Errenity.LogType = "error";
                Errenity.LogServerOrClient = "server";
                Errenity.Ip = Settings.CurrentIp;
                Errenity.LogMessage = Err;
                Errenity.SrcData = (SrcData==null? string.Empty:SrcData);
                Task.Run(() =>
                {
                    MongoDBBll.SetHost("log");
                    bool Flg = MongoDBBll.InsertLog(Errenity);
                });
            }
            catch (Exception err)
            { 
            }
        }
        public static void InfoLog(string LogProjecdtSrc,
            string LogApiClassSrc, string LogMothedSrc,
            object Message, string SrcData = null)
        {

            try
            {
                var Settings = MongoServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
                LogEntity Errenity = new LogEntity();
                Errenity._id = ObjectId.GenerateNewId();
                Errenity.LogProjecdtSrc = LogProjecdtSrc;
                Errenity.LogApiClassSrc = LogApiClassSrc;
                Errenity.LogMothedSrc = LogMothedSrc;
                Errenity.LogType = "info";
                Errenity.LogServerOrClient = "server";
                Errenity.Ip = Settings.CurrentIp;
                Errenity.LogMessage = Message;
                Errenity.SrcData = (SrcData == null ? string.Empty : SrcData);
                Task.Run(() =>
                {
                    MongoDBBll.SetHost("log");
                    bool Flg = MongoDBBll.InsertLog(Errenity);
                });
            }
            catch (Exception err)
            {
            }
        }
    }
}
