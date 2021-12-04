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

        public string ServiceName { get; set; }
    }
}
