using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 流程配置
    /// </summary>
    [Serializable]
    public class FlowConfig
    {
        /// <summary>
        /// 流程Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 工具集合
        /// </summary>
        public List<ToolContext> Tools { get; set; }
    }
}
