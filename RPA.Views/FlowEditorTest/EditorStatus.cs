using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Views.FlowEditor
{
    public enum EditorStatus
    {
        None,
        /// <summary>正在拖动中。</summary>
        Moving,
        /// <summary>正在绘制曲线中。</summary>
        Drawing,
        /// <summary>正在选择中。</summary>
        Selecting,
        /// <summary>多个选中正在拖动中。</summary>
        MultiMoving
    }
}
