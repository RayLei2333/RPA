using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Message
{
    /// <summary>
    /// 消息管理器
    /// </summary>
    public interface IMessage
    {

        /// <summary>
        /// 注册消息监听器。
        /// </summary>
        /// <param name="listener">消息监听者</param>
        void RegisterListener(MsgListener listener);
        /// <summary>
        /// 发送消息.
        /// </summary>
        /// <param name="msg">消息</param>
        bool PushMsg(Msg msg);
        /// <summary>
        /// 发送消息（同步模式）
        /// </summary>
        /// <param name="msg"></param>
        void PushSyncMsg(Msg msg);
        /// <summary>
        /// 获取由PushSyncMsg发送出的同步消息，如果没有消息，将一直Block住。
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="msTimeout">超时时间，默认-1为一直Block</param>
        /// <param name="cancelable">是否可取消</param>
        /// <returns>Msg体，如果没有将返回到Null</returns>
        Msg GetSyncMsg(string subject, int msTimeout = -1, bool cancelable = true);
        /// <summary>
        /// 启用同步信息任务.
        /// </summary>
        void StartSyncMsg();
        /// <summary>
        /// 取消同步消息任务.
        /// </summary>
        void CancelSyncMsg();
        /// <summary>
        /// 解绑消息监听器
        /// </summary>
        /// <param name="listener"></param>
        void UnRegisterListener(MsgListener listener);
        /// <summary>
        /// 解绑主题以及该主题下所有消息监听器
        /// </summary>
        /// <param name="subject"></param>
        void UnRegisterListener(string subject);
        void RegisterMsgHandle(Action<Msg> action);
        /// <summary>
        /// 销毁
        /// </summary>
        void Destroy();
    }
}
