using Framework.Manager;
using Framework.Utils;
using log4net;
using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Packets;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Framework.Message
{
    public class MQTTMsgManager : SingletonManager<MQTTMsgManager>, IMessage
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private IMqttClient _client = null;
        /// <summary>
        /// 消息监听者.
        /// </summary>
        private List<MsgListener> listeners = new List<MsgListener>();

        public string IP { get; set; }

        public int Port { get; set; }

        public bool ClientStart { get { return _client == null ? false : _client.IsConnected; } }

        private object _lock = new object();

        private Action<Msg> _msgHandle;

        /// <summary>
        /// 初始化MQTT连接及开启队列消息处理.
        /// </summary>
        public override void Init()
        {
            if (!ConfigHelperUtil.GetValue<bool>("EnableMQTT"))
                return;

            string serverAddress = ConfigHelperUtil.GetValue<string>("MQTTServerAddress");
            string[] arr = serverAddress.Split(':');
            IP = arr[0];
            Port = int.Parse(arr[1]);
            CreateClientAndStart(IP, Port, null, null);
        }


        /// <summary>
        /// 简易创建MQTTClient并运行
        /// </summary>
        /// <param name="mqttServerUrl">mqttServer的Url</param>
        /// <param name="port">mqttServer的端口</param>
        /// <param name="userName">认证用用户名</param>
        /// <param name="userPassword">认证用密码</param>
        /// <param name="callback">信息处理逻辑</param>
        /// <returns></returns>
        public async void CreateClientAndStart(string mqttServerUrl, int port, string userName, string userPassword)
        {
            try
            {
                MqttClientOptionsBuilder mqttClientOptionsBuilder = new MqttClientOptionsBuilder();
                mqttClientOptionsBuilder.WithTcpServer(mqttServerUrl, port);          // 设置MQTT服务器地址
                if (!string.IsNullOrEmpty(userName))
                {
                    mqttClientOptionsBuilder.WithCredentials(userName, userPassword);  // 设置鉴权参数
                }
                mqttClientOptionsBuilder.WithClientId(Guid.NewGuid().ToString("N"));  // 设置客户端序列号
                MqttClientOptions options = mqttClientOptionsBuilder.Build();
                if (_client != null)
                {
                    _client.Dispose();
                }
                _client = new MqttClientFactory().CreateMqttClient();
                _client.ConnectedAsync += ConnectedHandle;        // 服务器连接事件
                _client.DisconnectedAsync += DisconnectedHandle;  // 服务器断开事件（可以写入重连事件）
                _client.ApplicationMessageReceivedAsync += ApplicationMessageReceivedHandle;  // 发送消息事件
                await _client.ConnectAsync(options);  // 连接

                if (_client.IsConnected)
                {
                    _logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "开启MQTTClient成功！");
                }
                else
                {
                    _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "开启MQTTClient失败！");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "开启MQTTClient_失败！错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 关闭MQTTClient
        /// </summary>
        public async void DisconnectClientAsync()
        {
            try
            {
                if (_client != null && _client.IsConnected)
                {
                    await _client.DisconnectAsync();
                    _client.Dispose();
                    _client = null;
                    _logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "关闭MQTTClient成功！");
                }
                else
                {
                    _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "关闭MQTTClient_失败！MQTTClient未开启连接！");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "关闭MQTTClient_失败！错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 重连
        /// </summary>
        /// <returns></returns>
        public async void ReconnectClientAsync()
        {
            try
            {
                if (_client != null)
                {
                    await _client.ReconnectAsync();
                    _logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "MQTTClient重连_成功！");
                }
                else
                {
                    _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "MQTTClient重连_失败！未设置MQTTClient连接！");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "MQTTClient重连_失败！错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topic">主题</param>
        public async void SubscribeClientAsync(string topic)
        {
            try
            {
                MqttTopicFilter topicFilter = new MqttTopicFilterBuilder().WithTopic(topic).Build();
                await _client.SubscribeAsync(topicFilter, CancellationToken.None);
                _logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $":MQTTClient执行了订阅'{topic}'_成功！");
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $":MQTTClient执行了订阅'{topic}'_失败！错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 退订阅
        /// </summary>
        /// <param name="topic">主题</param>
        public async void UnsubscribeClientAsync(string topic)
        {
            try
            {
                await MqttClientExtensions.UnsubscribeAsync(_client, topic, CancellationToken.None);
                _logger.Info(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"MQTTClient执行了退订'{topic}'_成功！");
            }
            catch (Exception ex)
            {
                _logger.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + $"MQTTClient执行退订'{topic}'_失败！错误信息：" + ex.Message);
            }
        }

        /// <summary>
        /// 发布消息( 必须在成功连接以后才生效 )
        /// </summary>
        /// <param name="topic">主题</param>
        /// <param name="msg">信息</param>
        /// <param name="retained">是否保留</param>
        /// <returns></returns>
        public async Task PublishClientAsync(string topic, string msg, bool retained)
        {

            try
            {
                MqttApplicationMessageBuilder mqttApplicationMessageBuilder = new MqttApplicationMessageBuilder();
                mqttApplicationMessageBuilder.WithTopic(topic);          // 主题
                mqttApplicationMessageBuilder.WithPayload(msg);          // 信息
                mqttApplicationMessageBuilder.WithRetainFlag(retained);  // 保留

                MqttApplicationMessage messageObj = mqttApplicationMessageBuilder.Build();

                if (_client.IsConnected)
                {
                    await _client.PublishAsync(messageObj, CancellationToken.None);
                    _logger.Info($"发布MQTT消息成功！主题:'{topic}'，信息:'{msg}'，是否保留:'{retained}'");
                }
                else
                {
                    // 未连接
                    _logger.Error($"发布MQTT消息失败！ MQTTClient未开启连接！主题:'{topic}'，信息:'{msg}'，是否保留:'{retained}'");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"发布MQTT消息错误！主题:'{topic}'，信息:'{msg}'，是否保留:'{retained}'\"错误信息：" + ex.Message);
            }
        }


        #region Event
        /// <summary>
        /// 服务器连接事件
        /// </summary>
        private Task ConnectedHandle(MqttClientConnectedEventArgs arg)
        {
            MemoryMsgManager.Instance().PushMsg(new Msg("MQTTStatus", 1));
            lock (_lock)
            {
                List<string> subjects = new List<string>();
                foreach (var listener in listeners)
                {
                    if (!subjects.Contains(listener.Subject))
                    {
                        Subscribe(listener.Subject);
                        subjects.Add(listener.Subject);
                    }
                }
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 服务器断开事件（可以写入重连事件）
        /// </summary>
        private Task DisconnectedHandle(MqttClientDisconnectedEventArgs arg)
        {
            MemoryMsgManager.Instance().PushMsg(new Msg("MQTTStatus", 0));
            if (ClientStart)
            {
                _logger.Error($"The MQTT connected error.the result code is [{arg.Exception}]");
                Task.Delay(5000).ContinueWith(t =>
                {
                    ReconnectClientAsync();
                });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 发送消息事件
        /// </summary>
        private Task ApplicationMessageReceivedHandle(MqttApplicationMessageReceivedEventArgs arg)
        {
            var dataJson = Encoding.UTF8.GetString(arg.ApplicationMessage.Payload);
            Msg msg = JsonSerializer.Deserialize<Msg>(dataJson);
            //msg.Data = msg.Data.ToTEntity<MqttData>();
            lock (_lock)
            {
                foreach (var listener in listeners)
                {
                    if (string.Equals(listener.Subject, msg.Subject))
                    {
                        listener.Update(msg);
                    }
                }
            }

            return Task.CompletedTask;
        }
        #endregion
        /// <summary>
        /// 发送消息.
        /// </summary>
        /// <param name="msg">消息</param>
        public bool PushMsg(Msg msg)
        {
            if (!ClientStart)
                return false;
            PublishClientAsync(msg.Subject, msg, false);
            return true;
        }

        /// <summary>
        /// 注册消息监听器。
        /// </summary>
        /// <param name="listener">消息监听者</param>
        public void RegisterListener(MsgListener listener)
        {

            lock (_lock)
            {
                if (ClientStart)
                {
                    var list = listeners.Where(t => string.Equals(t.Subject, listener.Subject)).ToList();
                    if (list.Count == 0)
                    {
                        Subscribe(listener.Subject);
                    }
                    listeners.Add(listener);
                }
            }
        }


        /// <summary>
        /// 解绑主题下所有异常消息监听器
        /// </summary>
        /// <param name="subject">消息的主题</param>
        public void UnRegisterListener(MsgListener listener)
        {
            if (listeners != null)
            {
                if (listeners.Contains(listener))
                {
                    listeners.Remove(listener);
                }
                var list = listeners.Where(t => string.Equals(t.Subject, listener.Subject)).ToList();
                if (list.Count == 0)
                {
                    UnSubscribe(listener.Subject);
                }
            }
        }

        /// <summary>
        /// 解绑主题下所有异常消息监听器
        /// </summary>
        /// <param name="subject">消息的主题</param>
        public void UnRegisterListener(string subject)
        {
            lock (_lock)
            {
                if (ClientStart)
                {
                    List<MsgListener> tempList = listeners.FindAll(delegate (MsgListener ml) { return ml.Subject == subject; });
                    foreach (MsgListener li in tempList)
                    {
                        listeners.Remove(li);
                    }
                    UnSubscribe(subject);
                }
            }
        }

        public async void PublishClientAsync(string topic, object data, bool retained)
        {
            string msg = JsonSerializer.Serialize(data);
            await PublishClientAsync(topic, msg, retained);
        }

       
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="topic"></param>
        private void Subscribe(string topic)
        {
            if (ClientStart)
            {
                SubscribeClientAsync(topic);

            }
        }
        /// <summary>
        /// 取消订阅主题
        /// </summary>
        /// <param name="topic"></param>
        private void UnSubscribe(string topic)
        {
            if (ClientStart)
            {
                UnsubscribeClientAsync(topic);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            lock (_lock)
            {
                if (ClientStart)
                {

                    List<string> subjects = new List<string>();
                    foreach (var listener in listeners)
                    {
                        if (!subjects.Contains(listener.Subject))
                        {
                            UnSubscribe(listener.Subject);
                            subjects.Add(listener.Subject);
                        }
                    }
                    listeners.Clear();
                    DisconnectClientAsync();
                }
            }
        }

        public void RegisterMsgHandle(Action<Msg> action)
        {
            _msgHandle = action;
        }


        public void CancelSyncMsg()
        {
            throw new NotImplementedException();
        }

        public Msg GetSyncMsg(string subject, int msTimeout = -1, bool cancelable = true)
        {
            throw new NotImplementedException();
        }

        public void PushSyncMsg(Msg msg)
        {
            throw new NotImplementedException();
        }


        public void StartSyncMsg()
        {
            throw new NotImplementedException();
        }

    }
}
