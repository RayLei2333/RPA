using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace RPA.Controls.FlowEditor
{
    public class WrokflowEditor : Canvas
    {

        private NodeStream _nodeStream;

        public void InitNodeView(int x, int y, int regionWidth)
        {
            int startY = y;
            var nodeView = CreateNodeView(_nodeStream.StartNode);
            this.Children.Add(nodeView);
            Canvas.SetLeft(nodeView, x + (regionWidth - nodeView.Width) / 2);
            Canvas.SetTop(nodeView, startY);
            startY += 80;
            var parentView = nodeView;
            foreach (FlowNode node in _nodeStream.NodeList)
            {
                if (node is NodeBlock block)
                {
                    block.InitBlockView(this, x, startY, regionWidth, parentView);
                    startY += block.Height;
                }
                else
                {
                    nodeView = CreateNodeView(node);
                    this.Children.Add(nodeView);
                    Canvas.SetLeft(nodeView, x + (regionWidth - nodeView.Width) / 2);
                    Canvas.SetTop(nodeView, startY);
                    if (parentView != null)
                    {
                        //添加连线规则
                    }
                    parentView = nodeView;
                    startY += 80;
                }
            }
        }

        public WrokflowItem InitNodeView(int x, int y, int regionWidth, WrokflowItem parentView = null)
        {
            int startY = y;
            WrokflowItem parent = parentView;
            foreach (var node in _nodeStream.NodeList)
            {
                if (node is NodeBlock block)
                {
                    parent = block.InitBlockView(this, x, startY, regionWidth, parent);
                    startY += block.Height;
                }
                else
                {
                    var nodeView = CreateNodeView(node);
                    this.Children.Add(nodeView);
                    Canvas.SetLeft(nodeView, x + (regionWidth - nodeView.Width) / 2);
                    Canvas.SetTop(nodeView, startY);
                    if (parent != null)
                    {
                        //添加连线规则
                        //var arrowView = FlowCommon.CreateArrow(parentView, nodeView, 1);
                        //layout.Children.Add(arrowView);
                        //parentView.AddArrowView(arrowView);
                    }
                    parentView = nodeView;
                    startY += 80;
                }
            }

            return parent;
        }


        public WrokflowItem CreateNodeView(FlowNode node)
        {
            WrokflowItem item = new WrokflowItem(node); ;
            switch (node.NodeType)
            {
                case FlowNodeType.Start:
                    item.ShapeType = ShapeType.Circle;
                    break;
                case FlowNodeType.If:
                case FlowNodeType.Loop:
                    item.ShapeType = ShapeType.Diamond;
                    break;
                case FlowNodeType.End:
                    item.ShapeType = ShapeType.Ellipse;
                    break;
                case FlowNodeType.Default:
                default:
                    item.ShapeType=ShapeType.Rectangle;
                    break;
            }

            return item;
        }


    }
}
