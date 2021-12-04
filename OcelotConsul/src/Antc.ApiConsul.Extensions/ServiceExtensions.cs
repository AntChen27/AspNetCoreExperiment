using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Antc.ApiConsul.Extensions
{
    /// <summary>
    /// consul
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// 注册consul
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IApplicationLifetime lifetime, ConsulRegisterOptions options)
        {
            //consul地址
            Action<ConsulClientConfiguration> configClient = (consulConfig) =>
            {
                consulConfig.Address = new Uri(options.Address);
                consulConfig.Datacenter = options.Datacenter;
            };
            //建立连接
            var consulClient = new ConsulClient(configClient);
            //var httpCheck = new AgentServiceCheck()
            //{
            //    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
            //    Interval = TimeSpan.FromSeconds(10),//健康监测
            //    HTTP = options.HealthCheckUrl,//心跳检测地址
            //    Timeout = TimeSpan.FromSeconds(5)
            //};
            //注册
            var registrtion = new AgentServiceRegistration()
            {
                Checks = options.Checks,
                ID = options.ServiceName + "_" + Guid.NewGuid().ToString(),//服务编号不可重复
                Name = options.ServiceName,//服务名称
                Address = options.ServiceIp,//ip地址
                Port = options.ServicePort//端口
            };
            //注册服务
            consulClient.Agent.ServiceRegister(registrtion);


            consulClient.Agent.ServiceRegister(registrtion).Wait();//服务启动时注册，内部实现其实就是使用 Consul API 进行注册（HttpClient发起）

            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registrtion.ID).Wait();//服务停止时取消注册
            });

            return app;
        }

        /// <summary>
        /// 配置自动Consul注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureConsulAutoRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulRegisterOptions>(configuration.GetSection("Consul"));
            services.AddHostedService<ConsulRegisterHostService>();
            return services;
        }
    }
}
