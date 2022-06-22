using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xrpc_Beetlx
{
    public interface IsampleCap
    {
        Task<string> GetPage(string Obj);
        Task<string> Hello(string name);
    }
}
