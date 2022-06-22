using HelperTools;
using Microsoft.Extensions.DependencyInjection;
using rabbmq.AssemblyRegister;
using rabbmq.Class;
using rabbmq.Dbclass;
using rabbmq.Pageing.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.HangFireJob
{
    public class HangJobMethod
    {
        private int CashQue;
        public HangJobMethod()
        {
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            CashQue = Settings.QueCash;
        }

        /// <summary>
        /// 清除日志文件 删除3天以前  日志记录写入mongodb
        /// </summary>
        /// <returns></returns>
        public Task CleanLogFile()
        {
            DateTime CurrentTime = DateTime.Now;
            string Thistime = CurrentTime.AddHours(-10).ToString("u");
            MongoDBBll.SetHost("log");
            bool Flg=MongoDBBll.DeleteLOG(Thistime);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 清除API队列1分钟前对列
        /// </summary>
        /// <returns></returns>
        public Task CleanQueApi()
        {
            try
            {
                long CurrentTime = TimeSpans.Timestamp()-60000;//60秒之前
                switch (CashQue)
                {
                    case 1:
                        var Api = MyServiceProvider.ServiceProvider.GetRequiredService<ApiQueue>();
                        Api.ListQue.RemoveAll(c=>c.Timespan< CurrentTime);
                        break;
                    case 2:
                        MongoDBBll.SetHost("cap");
                        MongoDBBll.DeleteQUE(CurrentTime);
                        break;
                }
            }
            catch (Exception ex)
            { 
            }
            return Task.CompletedTask;
        }
        public async Task ClearSocket()
        {
            long CurrentTime = TimeSpans.Timestamp() - 600000;//10分钟
            var Settings = MyServiceProvider.ServiceProvider.GetRequiredService<Appsetings>();
            string MongoUrl = $"mongodb://lxzpower:123456@{Settings.SQLCfg.Mogon_Conn}/ownerconfig";
            MongoDbDal<WebSocketNetty>.GuestSetHost(MongoUrl, "ownerconfig", "socketCollection");
            DelFindUpFilter Del = new DelFindUpFilter();
            Del.FieldValue = CurrentTime.ToString();
            Del.FieldType = "long";
            Del.FieldName = "Timespan";
            Del.FieldFiler = "lte";
            Del.Mothed = "del";
            List<DelFindUpFilter> lDel = new List<DelFindUpFilter>();
            lDel.Add(Del);
            await MongoDbDal<WebSocketNetty>.DeleteManyFilterAsync(lDel);
            //return Task.CompletedTask; 
        }
    }
}
