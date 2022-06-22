using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq
{
    public static class MyServiceProvider
    {
        public static IServiceProvider ServiceProvider
        {
            get; set;
        }
        public static IServiceCollection Services
        {
            get; set;
        }
    }
}
