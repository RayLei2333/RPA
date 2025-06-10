using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Message
{
    /// <summary>
    /// 消息监听器
    /// </summary>
    public class MsgListener
    {
        /// <summary>
        /// 异步消息回调函数,由监听方实现.
        /// </summary>
        /// <param name="msg">消息体</param>
        public delegate void OnUpdate(Msg msg);

        private OnUpdate _onUpdate;

        /// <summary>
        /// 消息的主题.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 构造函数.
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="action">回调函数</param>
        public MsgListener(string subject, OnUpdate action)
        {
            Subject = subject;
            _onUpdate = action;
        }

        /// <summary>
        /// 更新消息.
        /// </summary>
        /// <param name="msg">消息体</param>
        public void Update(Msg msg)
        {
            _onUpdate(msg);
        }
    }
}
