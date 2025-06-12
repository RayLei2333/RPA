using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Views.ViewModels.FlowEdit
{
    internal class ToolModel
    {
        /// <summary>
        /// 工具名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 工具图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 工具类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 工具是否可用
        /// </summary>
        public bool Enable { get; set; }
    }
}
