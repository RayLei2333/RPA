using Framework.Manager;
using Framework.Utils;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override void Init()
        {
            if (!ConfigHelperUtil.GetValue<bool>("EnableMQTT"))
                return;

            string serverAddress = ConfigHelperUtil.GetValue<string>("MQTTServerAddress");
            string[] arr = serverAddress.Split(':');
            IP = arr[0];
            Port = int.Parse(arr[1]);
        }


        public void CancelSyncMsg()
        {
            throw new NotImplementedException();
        }

        public Msg GetSyncMsg(string subject, int msTimeout = -1, bool cancelable = true)
        {
            throw new NotImplementedException();
        }

        public bool PushMsg(Msg msg)
        {
            throw new NotImplementedException();
        }

        public void PushSyncMsg(Msg msg)
        {
            throw new NotImplementedException();
        }

        public void RegisterListener(MsgListener listener)
        {
            throw new NotImplementedException();
        }

        public void RegisterMsgHandle(Action<Msg> action)
        {
            throw new NotImplementedException();
        }

        public void StartSyncMsg()
        {
            throw new NotImplementedException();
        }

        public void UnRegisterListener(MsgListener listener)
        {
            throw new NotImplementedException();
        }

        public void UnRegisterListener(string subject)
        {
            throw new NotImplementedException();
        }
    }
}
