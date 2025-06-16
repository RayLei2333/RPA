using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Views.Models.FlowEdit
{
    public class ToolGroupModel
    {
        /// <summary>
        /// 工具分组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 分组是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 分组下的工具集合
        /// </summary>
        public List<ToolModel> Tools { get; set; }
    }
}
