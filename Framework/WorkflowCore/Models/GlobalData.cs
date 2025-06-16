using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 流程全局变量
    /// </summary>
    public class GlobalData : GlobalVar
    {
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }
    }
}
