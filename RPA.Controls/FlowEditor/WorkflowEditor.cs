using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace RPA.Controls.FlowEditor
{
    public class WorkflowEditor : Canvas
    {

        public NodeStream NodeStream { get; set; }

        private Dictionary<string, FlowNode> _flowNodes;

        public void SetFlowNodes(Dictionary<string, FlowNode> flowNodes)
        {
            _flowNodes = flowNodes;


            GenerateFlowModule();
        }

        /// <summary>
        /// 流程模块划分
        /// </summary>
        private void GenerateFlowModule()
        {
            var startNode = _flowNodes["start"];
            var end = _flowNodes["end"];
            NodeStream = new NodeStream();
            NodeStream.SetStartEndNode(startNode, end, _flowNodes);
            NodeStream.CalcSize();
        }


        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">添加的节点</param>
        /// <param name="parentId">父节点Id</param>
        /// <param name="childId">子节点Id</param>
        public void AddFlowNode(FlowNode node, string parentId, string childId)
        {
            var parentNode = _flowNodes[parentId];
            if (parentNode.NodeType == FlowNodeType.End)
                return;
            var childNode = _flowNodes[childId];

            _flowNodes.Add(node.Id, node);
            parentNode.ReplaceSubNode(childNode, node);
            node.Childs.Add(childNode);
            node.Parent = parentNode;
            childNode.Parent = node;
        }

        /// <summary>
        /// 初始化节点
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="regionWidth"></param>
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

        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public WorkflowItem CreateNodeView(FlowNode node)
        {
            WorkflowItem item = new WorkflowItem();
            item.SetNode(node);
            item.ShapeType = node.ShapeType;
            item.FlowNodeType = node.NodeType;
            return item;
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="args"></param>
        public object GetNode(Point point)
        {
            foreach (var child in this.Children)
            {
                if (child is ArrowView arrowView)
                {
                    if (arrowView.Contain(point))
                        return child;
                }

                if (child is WorkflowItem workflowItem)
                {
                    if (workflowItem.Contain(point))
                        return workflowItem;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取父节点和子节点Id
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public string[] GetAroundId(object view)
        {
            string parentId = null,
                   childId = null;

            if (view is ArrowView arrowView)
            {
                parentId = arrowView.FromNode.FlowNode.Id;
                childId = arrowView.ToNode.FlowNode.Id;
            }

            if (view is WorkflowItem workflowItem)
            {
                if (workflowItem.FlowNode.NodeType == FlowNodeType.End)
                    return null;
                parentId = workflowItem.FlowNode.Id;

                if (workflowItem.FlowNode.Childs.Count > 0)
                    childId = workflowItem.FlowNode.Childs[0].Id;

            }


            return new string[] { parentId, childId };
        }


        /// <summary>
        /// 刷新布局
        /// </summary>
        public void RefreshLayout()
        {
            this.Children.Clear();
            GenerateFlowModule();
            InitNodeView(0, 0, 0);
        }
    }
}
