using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 工具参数
    /// </summary>
    [Serializable]
    public class ToolParam : GlobalVar
    {
        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }
    }
}
