using DotNetCore.CAP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq
{
    public class CapUserService : ICapSubscribe, ICapUserService
    {
        [CapSubscribe("m.test")]
        public void SubscribeWithnoController(string date)
        {
            Console.WriteLine($"SubscribeWithnoController接收到订阅:{date}");
        }
    }
}
