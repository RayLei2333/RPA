using Framework.Manager;
using Framework.Utils;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Message
{
    /// <summary>
    /// MQTT 服务端
    /// </summary>
    public class MQTTServerManager : SingletonManager<MQTTServerManager>
    {
        /// <summary>
        /// MQTT服务
        /// </summary>
        private MqttServer _server = null;

        /// <summary>
        /// 服务是否启动
        /// </summary>
        public bool ServerStart { get { return _server == null ? false : _server.IsStarted; } }

        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Port { get; set; }

        public override void Init()
        {
            base.Init();
            if (!ConfigHelperUtil.GetValue<bool>("EnableMQTT") || !ConfigHelperUtil.GetValue<bool>("IsMQTTServer"))
                return;
            string serverAddress = ConfigHelperUtil.GetValue<string>("MQTTServerAddress");
            string[] arr = serverAddress.Split(':');
            IP = arr[0];
            Port = int.Parse(arr[1]);
            CreateServerAndStart(IP, Port);
        }

        public async void CreateServerAndStart(string ip, int port, bool withPersistentSessions = true)
        {
            try
            {
                MqttServerOptionsBuilder mqttServerOptionsBuilder = new MqttServerOptionsBuilder();  // MQTT服务器配置
                mqttServerOptionsBuilder.WithDefaultEndpoint();
                mqttServerOptionsBuilder.WithDefaultEndpointBoundIPAddress(IPAddress.Parse(ip));  // 设置Server的IP
                mqttServerOptionsBuilder.WithDefaultEndpointPort(port);                           // 设置Server的端口号
                mqttServerOptionsBuilder.WithPersistentSessions(withPersistentSessions);  // 持续会话
                mqttServerOptionsBuilder.WithConnectionBacklog(2000);                     // 最大连接数

                MqttServerOptions mqttServerOptions = mqttServerOptionsBuilder.Build();
                _server = new MqttServerFactory().CreateMqttServer(mqttServerOptions);

                await _server.StartAsync();
                if (_server.IsStarted)
                {
                    _logger.Info("MQTT Server启动成功");
                }
                else
                {
                    _logger.Error("MQTT Server启动失败");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("MQTT Server启动失败", ex);
            }

        }

        public async void StopServer()
        {
            try
            {
                if (_server == null)
                {
                    _logger.Info("关闭MQTT Server出错！MQTTServer未在运行。");
                    return;
                }

                var clients = await _server.GetClientsAsync();
                foreach (var client in clients)
                {
                    await client.DisconnectAsync();
                }
                await _server.StopAsync();
                _server.Dispose();
                _server = null;
                _logger.Info("关闭MQTT Server结束！");
            }
            catch (Exception ex)
            {
                _logger.Error("关闭MQTT Server出错！", ex);
            }
        }


        public override void Destroy()
        {
            base.Destroy();
            StopServer();
        }
    }
}
