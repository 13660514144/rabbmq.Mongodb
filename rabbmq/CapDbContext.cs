using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq
{
    public class CapDbContext: DbContext
    {
        public CapDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
