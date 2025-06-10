using Framework.Manager;
using log4net;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Message
{
    /// <summary>
    /// 内存消息关系器
    /// </summary>
    public class MemoryMsgManager : SingletonManager<MemoryMsgManager>, IMessage
    {
        /// <summary>
        /// 最大队列数.
        /// </summary>
        private const int _maxQueueCount = 100;
        /// <summary>
        /// 异步消息队列.
        /// </summary>
        private BlockingCollection<Msg> _queue = new BlockingCollection<Msg>(_maxQueueCount);

        /// <summary>
        /// 同步消息队列.
        /// </summary>
        private Dictionary<string, BlockingCollection<Msg>> _syncQueueD = new Dictionary<string, BlockingCollection<Msg>>();

        /// <summary>
        /// 消息监听者.
        /// </summary>
        private List<MsgListener> _listeners = new List<MsgListener>();

        /// <summary>
        /// 同步消息取消令牌.
        /// </summary>
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private object _lock = new object();

        private bool _isAlive;

        private Action<Msg> _msgHandle;

        public override void Init()
        {
            Task.Factory.StartNew(() =>
            {
                while (_isAlive)
                {
                    try
                    {

                        Msg msg = _queue.Take();
                        NotifyListeners(msg);
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnFormat($"Task exec error {ex.StackTrace}");
                    }
                }
            }, TaskCreationOptions.LongRunning);

        }

        public override void Destroy()
        {
            _isAlive = false;
        }

        /// <summary>
        /// 注册消息监听器。
        /// </summary>
        /// <param name="listener">消息监听者</param>
        public void RegisterListener(MsgListener listener)
        {
            _listeners.Add(listener);
        }

        /// <summary>
        /// 解绑异步消息监听器。
        /// </summary>
        /// <param name="listener">消息监听者</param>
        public void UnRegisterListener(MsgListener listener)
        {
            if (listener == null)
            {
                return;
            }
            if (_listeners.Contains(listener))
            {
                _listeners.Remove(listener);
            }
        }

        /// <summary>
        /// 解绑主题下所有异常消息监听器
        /// </summary>
        /// <param name="subject">消息的主题</param>
        public void UnRegisterListener(string subject)
        {
            List<MsgListener> tempList = _listeners.FindAll(t => t.Subject == subject);
            foreach (MsgListener li in tempList)
            {
                _listeners.Remove(li);
            }
        }


        /// <summary>
        /// 推送异步消息.
        /// </summary>
        /// <param name="msg">消息</param>
        public bool PushMsg(Msg msg)
        {
            if (_queue.Count == _queue.BoundedCapacity)
            {
                _logger.Error($"Async msg:{msg.Subject} queue has expired capacity:{_queue.BoundedCapacity}, will abandon the msg.");
                return false;
            }
            _queue.Add(msg);
            return true;
        }

        /// <summary>
        /// 推送同步消息（推理过程中的同步消息，如果别的消息使用该队列，在停止推理时有可能被清除）,使用相同的GetSyncMsg接收消息.
        /// </summary>
        /// <param name="msg">消息</param>
        public void PushSyncMsg(Msg msg)
        {
            // 如果异步任务已取消，将不能推送消息
            if (_cts.IsCancellationRequested)
            {
                _logger.Warn($"Async msg have not been cancel, will not received the msg: {msg.Subject}.");
            }

            if (!_syncQueueD.ContainsKey(msg.Subject))
            {
                CreateSyncMsgSubject(msg.Subject);
            }

            if (_syncQueueD[msg.Subject].Count == _syncQueueD[msg.Subject].BoundedCapacity)
            {
                _logger.Error($"Sync msg:{msg.Subject} queue has expired max:{_maxQueueCount}, will block produce msg thread.");

                return;
            }

            _syncQueueD[msg.Subject].Add(msg);
        }

        /// <summary>
        /// 获取由PushSyncMsg发送出的同步消息，如果没有消息，将一直Block住。
        /// </summary>
        /// <param name="subject">主题</param>
        /// <param name="msTimeout">超时时间，默认-1为一直Block</param>
        /// <param name="cancelable">是否可取消</param>
        /// <returns>Msg体，如果没有将返回到Null</returns>
        public Msg GetSyncMsg(string subject, int msTimeout = -1, bool cancelable = true)
        {
            // 建立主题
            if (!_syncQueueD.ContainsKey(subject))
            {
                CreateSyncMsgSubject(subject);
            }

            Msg msg = null;
            try
            {
                bool result = cancelable
                    ? _syncQueueD[subject].TryTake(out msg, msTimeout, _cts.Token)
                    : _syncQueueD[subject].TryTake(out msg, msTimeout);

                if (!result)
                {
                    _logger.Warn($"Msg：{subject} take false.");
                }
            }
            catch (Exception e)
            {
                _logger.Info($"Cancel sync msg {subject}.");
            }

            return msg;
        }

        private void CreateSyncMsgSubject(string subject)
        {
            lock (_lock)
            {
                if (!_syncQueueD.ContainsKey(subject))
                {
                    _logger.Info($"Create sync msg subject [{subject}]");
                    _syncQueueD[subject] = new BlockingCollection<Msg>(_maxQueueCount);
                }
            }
        }

        private void NotifyListeners(Msg msg)
        {
            var temp = _listeners.Where(t => string.Equals(t.Subject, msg.Subject)).ToList();//遍历快照
            Parallel.ForEach(temp, listener =>
            {
                listener.Update(msg);
            });
        }

        /// <summary>
        /// 启用异步信息任务.
        /// </summary>
        public void StartSyncMsg()
        {
            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// 取消异步消息任务.
        /// </summary>
        public void CancelSyncMsg()
        {
            _cts.Cancel();

            foreach (var key in _syncQueueD.Keys)
            {
                _syncQueueD[key].CompleteAdding();
                _syncQueueD[key].Dispose();
            }
            _syncQueueD.Clear();
        }

        public void RegisterMsgHandle(Action<Msg> action)
        {
            _msgHandle = action;
        }
    }
}
