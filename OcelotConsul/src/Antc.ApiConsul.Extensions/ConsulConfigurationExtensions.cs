using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Winton.Extensions.Configuration.Consul;

namespace Antc.ApiConsul.Extensions
{
    public static class ConsulConfigurationExtensions
    {
        /// <summary>
        /// 添加Consul 配置中心做为配置源
        /// <para>
        /// <br/>
        /// Key / Values 配置路径
        /// <br/>
        /// 通用配置路径 conf/appsettings.json
        /// <br/>
        /// 当前服务专用配置路径 conf/{ServiceName}/appsettings.json
        /// <br/>
        /// 说明：ServiceName名称见项目appsettings.json 中的Consul:ServiceName配置节点
        /// </para>
        /// </summary>
        /// <param name="hostingContext"></param>
        /// <param name="configurationBuilder"></param>
        public static void AddConsulConfiguration(HostBuilderContext hostingContext, IConfigurationBuilder configurationBuilder)
        {
            var env = hostingContext.HostingEnvironment;
            hostingContext.Configuration = configurationBuilder.Build();
            var consulUrl = hostingContext.Configuration["Consul:Address"];
            var serviceName = hostingContext.Configuration["Consul:ServiceName"];
            configurationBuilder.AddConsul(
                    $"conf/appsettings.json",
                    options =>
                    {
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulUrl); };
                    }
                )
                .AddConsul(
                    $"conf/{serviceName}/appsettings.json",
                    options =>
                    {
                        options.Optional = true;
                        options.ReloadOnChange = true;
                        options.OnLoadException = exceptionContext => { exceptionContext.Ignore = true; };
                        options.ConsulConfigurationOptions = cco => { cco.Address = new Uri(consulUrl); };
                    }
                );

            //获取配置的时候按照Add顺序倒序查，直到获取到

            hostingContext.Configuration = configurationBuilder.Build();
        }
    }
}
