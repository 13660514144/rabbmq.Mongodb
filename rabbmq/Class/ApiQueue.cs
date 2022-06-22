using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Class
{
    public class ApiQueue
    {
        public ApiQueue()
        {
            Que = new Models();
            ListQue = new List<Models>();
        }
        public Models Que;
        public List<Models> ListQue;
    }
    public class Models
    { 
        public string Guid { get; set; }
        public object Connect { get; set; } = string.Empty;
        public bool ExistValue { get; set; } = false;
        public long Timespan { get; set; }
    }
}
