using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RPA.Controls.FlowEditor
{
    public class WrokflowItem : ContentControl
    {
        public ShapeType ShapeType { get; set; }

        public FlowNode FlowNode { get; set; }

        public WrokflowItem(FlowNode node)
        {
            FlowNode = node;
        }
    }
}
