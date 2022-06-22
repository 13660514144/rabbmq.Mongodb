using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xrpc_Beetlx.Models
{
    public class XrpcReturn
    {
        public class RpModels
        {
            public int code { get; set; } = 200;
            public string message { get; set; } = string.Empty;
            public object data { get; set; }
            public object fieldsinfo { get; set; }
        }
    }
}
