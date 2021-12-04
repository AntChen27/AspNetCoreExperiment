using Consul;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Antc.ApiConsul.Extensions
{
    public class ConsulRegisterHostService : IHostedService
    {
        private readonly ConsulRegisterOptions _consulRegisterOptions;
        private ConsulClient _consulClient;
        private AgentServiceRegistration _registration;

        public ConsulRegisterHostService(IOptions<ConsulRegisterOptions> consulRegisterOptions)
        {
            _consulRegisterOptions = consulRegisterOptions.Value;
        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //consul地址
            Action<ConsulClientConfiguration> configClient = (consulConfig) =>
            {
                consulConfig.Address = new Uri(_consulRegisterOptions.Address);
                consulConfig.Datacenter = _consulRegisterOptions.Datacenter;
            };
            //建立连接
            _consulClient = new ConsulClient(configClient);
            //var httpCheck = new AgentServiceCheck()
            //{
            //    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
            //    Interval = TimeSpan.FromSeconds(10),//健康监测
            //    HTTP = options.HealthCheckUrl,//心跳检测地址
            //    Timeout = TimeSpan.FromSeconds(5)
            //};
            //注册
            _registration = new AgentServiceRegistration()
            {
                Checks = _consulRegisterOptions.Checks,
                ID = _consulRegisterOptions.ServiceName + "_" + Guid.NewGuid().ToString(),//服务编号不可重复
                Name = _consulRegisterOptions.ServiceName,//服务名称
                Address = _consulRegisterOptions.ServiceIp,//ip地址
                Port = _consulRegisterOptions.ServicePort//端口
            };
            //注册服务
            _consulClient.Agent.ServiceRegister(_registration).Wait(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consulClient != null && _registration != null)
            {
                _consulClient.Agent.ServiceDeregister(_registration.ID).Wait(cancellationToken);
            }
        }
    }
}
