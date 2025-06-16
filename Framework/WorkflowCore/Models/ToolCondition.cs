using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 工具条件
    /// </summary>
    [Serializable]
    public class ToolCondition
    {
        /// <summary>
        /// 条件表达式
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// 满足条件后的调用目标
        /// </summary>
        public string Target { get; set; }
    }
}
