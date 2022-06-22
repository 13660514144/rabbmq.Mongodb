using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rabbmq.Signalr
{
    public class SignAlrUser
    {
        public SignAlrUser(string ConnectionID, string Id, string Name,string Type)
        {
            this.ConnectionID = ConnectionID;
            this.GuId = Id;
            this.Name = Name;
            this.Type = Type;
        }

        /// <summary>
        /// 连接ID
        /// </summary>
        [Key]
        public string ConnectionID { get; set; }

        public HubCallerContext Context { get; set; }

        public IHubCallerClients Clients { get; set; }

        public string GuId { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
    }
}
