using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace RPA.Controls.FlowEditor
{
    public class NodeBlock : FlowNode
    {
        private List<NodeStream> _streamList = new List<NodeStream>();

        private FlowNode _startNode;

        private FlowNode _endNode;

        public int Height { get; set; }

        public int Width { get; set; }

        public void InitBlock(FlowNode startNode, FlowNode endNode, Dictionary<string, FlowNode> allNodes)
        {
            _startNode = startNode;
            _endNode = endNode;
            foreach (var node in startNode.Childs)
            {
                var stream = new NodeStream();
                _streamList.Add(stream);
                var nextNode = node;
                while (!string.Equals(nextNode.Id, _endNode.Id))
                {
                    nextNode = stream.AddNode(nextNode, allNodes);
                }
            }
        }

        public void CalcSize()
        {
            Height = 80 * 2;
            int streamH = 0;
            int streamsW = 0;
            foreach (var stream in _streamList)
            {
                stream.CalcSize();
                if (streamH < stream.Height)
                {
                    streamH = stream.Height;
                }
                if (streamsW == 0)
                {
                    streamsW += stream.Width;
                }
                else
                {
                    streamsW += (stream.Width + 30);
                }
            }
            Height += streamH;
            Width = streamsW;
        }

        public WorkflowItem InitBlockView(WorkflowEditor editor, int x, int y, int regionWidth, WorkflowItem parent)
        {
            int startY = y,
                startX = x;
            var nodeView = editor.CreateNodeView(_startNode);
            editor.Children.Add(nodeView);
            Canvas.SetLeft(nodeView, x +(regionWidth - nodeView.Width) / 2);
            Canvas.SetTop(nodeView, startY);
            //添加连线规则
            if (parent != null)
            {
                editor.AddArrowView(parent, nodeView);
            }
            parent = nodeView;
            startY += (int)nodeView.Margin.Bottom;
            int height = 0,
                totalWidth = 0;
            for (int i = 0; i < _streamList.Count; i++)
            {
                totalWidth = i == 0 ? _streamList[i].Width : totalWidth + _streamList[i].Width + (int)nodeView.Margin.Right;
            }
            startX += (regionWidth - totalWidth) / 2;
            List<WorkflowItem> list = new List<WorkflowItem>();
            foreach (var stream in _streamList)
            {
                list.Add(editor.InitNodeView(startX, startY, regionWidth, parent));
                startX += stream.Width + 30;
                if(stream.Height > height)
                    height = stream.Height;
            }
            startY += height;

            var endNodeView = editor.CreateNodeView(_endNode);
            editor.Children.Add(endNodeView);
            Canvas.SetLeft(nodeView, x +(regionWidth - nodeView.Width) / 2);
            Canvas.SetTop(nodeView, startY);

            foreach (var item in list)
            {
                //创建连线规则
                editor.AddArrowView(item, endNodeView);
            }

            return endNodeView;

        }
    }
}
