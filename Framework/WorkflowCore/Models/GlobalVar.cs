using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 全局变量
    /// </summary>
    [Serializable]
    public class GlobalVar
    {
        /// <summary>
        /// 变量名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 列表中数据类型
        /// </summary>
        public string ListType { get; set; }
    }
}
