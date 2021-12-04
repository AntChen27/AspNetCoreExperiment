using System;
using System.Collections.Generic;
using System.Text;
using Consul;

namespace Antc.ApiConsul.Extensions
{
    public class ConsulRegisterOptions
    {
        /// <summary>
        /// Consul 地址 http://127.0.0.1:8500
        /// </summary>
        public string Address { get; set; }

        public string Datacenter { get; set; } = "dc1";

        public string ServiceName { get; set; }
        public string ServiceIp { get; set; }
        public int ServicePort { get; set; }

        public AgentServiceCheck[] Checks { get; set; }
    }
}
