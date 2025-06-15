using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace RPA.Controls.FlowEditor
{
    public class WorkflowEditor : Canvas
    {

        public NodeStream NodeStream { get; set; }

        public void SetFlowNodes(Dictionary<string, FlowNode> flowNodes)
        {
            var startNode = flowNodes["start"];
            var end = flowNodes["end"];
            NodeStream = new NodeStream();
            NodeStream.SetStartEndNode(startNode, end, flowNodes);
            NodeStream.CalcSize();

        }

        public void InitNodeView(int x, int y, int regionWidth)
        {
            regionWidth = (int)this.ActualWidth;
            int startY = y;
            var nodeView = CreateNodeView(NodeStream.StartNode);
            this.Children.Add(nodeView);
            Canvas.SetLeft(nodeView, x + (regionWidth - nodeView.Width) / 2);
            Canvas.SetTop(nodeView, startY);
            startY += (int)nodeView.Margin.Bottom;
            var parentView = nodeView;
            foreach (FlowNode node in NodeStream.NodeList)
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
                        AddArrowView(parentView, nodeView);
                    }
                    parentView = nodeView;
                    startY += (int)nodeView.Margin.Bottom;
                }
            }

            nodeView = CreateNodeView(NodeStream.EndNode);
            this.Children.Add(nodeView);
            Canvas.SetLeft(nodeView, x + (regionWidth - nodeView.Width) / 2);
            Canvas.SetTop(nodeView, startY);
            if (parentView != null)
            {
                AddArrowView(parentView, nodeView);
            }
        }

        public WorkflowItem InitNodeView(int x, int y, int regionWidth, WorkflowItem parentView = null)
        {
            int startY = y;
            WorkflowItem parent = parentView;
            foreach (var node in NodeStream.NodeList)
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
                        AddArrowView(parent, nodeView);
                    }
                    parentView = nodeView;
                    startY += (int)nodeView.Margin.Bottom;
                }
            }

            return parent;
        }

        /// <summary>
        /// 创建连线
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="child">子节点</param>
        /// <param name="lineStyle">线条样式</param>
        public void AddArrowView(WorkflowItem parent, WorkflowItem child, int lineStyle = 1)
        {
            ArrowView arrow = new ArrowView();
            arrow.SetLineStyle(lineStyle);
            arrow.X1 = Canvas.GetLeft(parent) + parent.Width / 2;
            arrow.Y1 = Canvas.GetTop(parent) + parent.Height;
            arrow.X2 = Canvas.GetLeft(child) + child.Width / 2;
            arrow.Y2 = Canvas.GetTop(child) - 1;
            arrow.Stroke = new SolidColorBrush(Color.FromRgb(0, 153, 255));
            arrow.StrokeThickness = 2;
            arrow.HeadWidth = 4;
            arrow.HeadHeight = 3;
            arrow.FromNode = parent;
            arrow.ToNode = child;
            this.Children.Add(arrow);
            parent.AddArrow(arrow);

        }

        public WorkflowItem CreateNodeView(FlowNode node)
        {
            WorkflowItem item = new WorkflowItem();
            item.SetNode(node);
            item.ShapeType = node.ShapeType;
            item.FlowNodeType = node.NodeType;
            return item;
        }


    }
}
