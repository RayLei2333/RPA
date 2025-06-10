using Framework.Message;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Manager
{
    public abstract class BaseManager<T>
    {

        protected ILog _logger = LogManager.GetLogger(typeof(T));

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init()
        {
            //mLogger.Debug($"{GetType()} has been init.");
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Destroy()
        {
            //mLogger.Debug($"{GetType()} has been destroy.");
        }

    }
}
