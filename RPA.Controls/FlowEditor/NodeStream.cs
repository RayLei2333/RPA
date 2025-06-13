using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace RPA.Controls.FlowEditor
{
    /// <summary>
    /// 节点块
    /// </summary>
    public class NodeStream
    {
        public FlowNode StartNode { get; set; }

        public FlowNode EndNode { get; set; }

        public List<FlowNode> NodeList = new List<FlowNode>();

        public int Width { get; set; }

        public int Height { get; set; }

        public void SetStartEndNode(FlowNode startNode, FlowNode endNode, Dictionary<string, FlowNode> allNodes)
        {
            StartNode = startNode;
            EndNode = endNode;
            var subNode = startNode.Childs.First();
            while (subNode.Id != EndNode.Id)
            {
                subNode = AddNode(subNode, allNodes);
            }

        }

        public FlowNode AddNode(FlowNode flowNode, Dictionary<string, FlowNode> allNodes)
        {
            //如果子元素节点数量大于1  按分块处理则分块
            if (flowNode.Childs.Count > 1)
            {
                NodeBlock nodeBlock = new NodeBlock();
                var endNode = allNodes[flowNode.Id.Replace("start", "end")];
                nodeBlock.InitBlock(flowNode, endNode, allNodes);
                NodeList.Add(nodeBlock);
                return endNode.Childs.FirstOrDefault();
            }
            else
            {
                NodeList.Add(flowNode);
                return flowNode.Childs.FirstOrDefault();
            }
        }

        /// <summary>
        /// 计算对应的高宽
        /// </summary>
        public void CalcSize()
        {
            foreach (var node in NodeList)
            {
                if(node is NodeBlock block)
                {
                    block.CalcSize();
                    int blockW = block.Width;
                    if (blockW > Width)
                    {
                        Width = blockW;
                    }
                    Height += block.Height;

                }
                else
                {
                    Height += 80;
                }
            }
        }


    }
}
