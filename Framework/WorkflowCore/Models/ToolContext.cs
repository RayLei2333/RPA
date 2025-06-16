using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore.Models
{
    /// <summary>
    /// 工具上下文
    /// </summary>
    [Serializable]
    public class ToolContext
    {
        /// <summary>
        /// 工具Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 工具类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 工具名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工具的条件
        /// </summary>
        public List<ToolCondition> Conditions { get; set; }

        /// <summary>
        /// 输入参数
        /// </summary>
        public List<ToolParam> InputParams { get; set; }

        /// <summary>
        /// 输出参数
        /// </summary>
        public List<ToolParam> OutputParams { get; set; }

        /// <summary>
        /// 常量参数
        /// </summary>
        public List<ToolParam> OptionParams { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 子节点Id
        /// </summary>
        public List<string> ChildrenIdList { get; set; } = new List<string>();

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 是否设置断点
        /// </summary>
        public bool IsBreakpoint { get; set; }
    }
}
