using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.Controls.FlowEditor
{
    public class FlowNode
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public FlowNodeType NodeType { get; set; }

        public ShapeType ShapeType { get; set; }

        public FlowNode Parent { get; set; }

        public List<FlowNode> Childs { get; set; } = new List<FlowNode>();


        public void ReplaceSubNode(FlowNode oldNode, FlowNode newNode)
        {
            int index = Childs.IndexOf(oldNode);
            if (index != -1)
            {
                Childs[index] = newNode;
            }
            else
            {
                Childs.Add(newNode);
            }
        }

    }
}
