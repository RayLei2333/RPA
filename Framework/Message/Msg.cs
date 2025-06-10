using Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Message
{
    /// <summary>
    /// 消息对象
    /// </summary>
    [Serializable]
    public class Msg
    {

        /// <summary>
        /// 消息主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        private object _replyData;
        private object _lock = new object();

        public Msg(string subject, object data)
        {
            Subject = subject;
            Data = data;
            Time = DateTime.Now;
        }

        public T GetData<T>()
        {
            return (T)Data;
        }

        public void Reply(object data)
        {
            lock (_lock)
            {
                _replyData = data;
                Monitor.Pulse(_lock);
            }
        }

        public T GetReplyData<T>(int timeout = -1)
        {
            lock (_lock)
            {
                if (_replyData != null)
                {
                    return (T)_replyData;
                }
                if (timeout == -1)
                {
                    Monitor.Wait(_lock);
                }
                else
                {
                    Monitor.Wait(_lock, timeout);
                }

                return (T)_replyData;
            }
        }
    }
}
