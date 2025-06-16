using Framework.WorkflowCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.WorkflowCore
{
    /// <summary>
    /// 工具接口
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// 工具上下文
        /// </summary>
        ToolContext ToolContext { get; }




        /// <summary>
        /// 是否带条件
        /// </summary>
        bool IsCondition { get; set; }

        void Init(Workflow workflow, ToolContext toolContext);
    }
}
