using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Manager
{
    public abstract class SingletonManager<T> : BaseManager<T>
    {
        /// <summary>
        /// 单例
        /// </summary>
        /// <returns></returns>
        public static T Instance()
        {
            return InstanceHolder.INSTANCE;
        }

        private static class InstanceHolder
        {
            public static T INSTANCE = System.Activator.CreateInstance<T>();
        }
    }
}
