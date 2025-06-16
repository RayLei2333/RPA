using RPA.Controls.FlowEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RPA.Views.Views.FlowEdit
{
    /// <summary>
    /// FlowEditWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FlowEditWindow : Window
    {
        public FlowEditWindow()
        {
            InitializeComponent();

            //this.toolListView.ToolMoveHandler += ToolListView_ToolMoveHandler;
            //this.toolListView.ToolUpHandler += ToolListView_ToolUpHandler;

            //this.toolListView.ToolDownHandler += ToolListView_ToolDownHandler;
            this.toolListView.ToolUpHandler += ToolListView_ToolUpHandler;
            this.toolListView.ToolMoveHandler += ToolListView_ToolMoveHandler;

        }

        private ToolView _moveToolView;

        private void ToolListView_ToolMoveHandler(object sender, MouseEventArgs args)
        {
            // Handle tool move logic here
            var view = sender as ToolView;
            if (view != null)
            {

                if (_moveToolView == null)
                {
                    _moveToolView = view.Clone();
                    ContentCanvas.Children.Add(_moveToolView);
                }
                var point = args.GetPosition(ContentCanvas);
                Canvas.SetLeft(_moveToolView, point.X);
                Canvas.SetTop(_moveToolView, point.Y);
            }
        }

        private void ToolListView_ToolUpHandler(object sender, MouseButtonEventArgs args)
        {
            if (_moveToolView == null)
                return;

            ContentCanvas.Children.Remove(_moveToolView);
            var point = args.GetPosition(this.FlowEditor);
            var view = this.FlowEditor.GetNode(point);
            if(view != null)
            {
                string[] aroundId = this.FlowEditor.GetAroundId(view);
                if(aroundId != null)
                {
                    string parentId = aroundId[0];
                    string childId = aroundId[1];

                    //添加工具列表
                    //var tools = CurFlow.FlowTools.InsertNewTool(MoveToolView.GetToolType(), MoveToolView.GetToolName(), parentId, childId);
                    //foreach (var tool in tools)
                    //{
                    //    //通过tool 创建FlowNode
                    //    FlowNode node = null; //CreateNodeView(tool);
                    //    this.FlowEditor.AddFlowNode(node, parentId, childId);
                    //    //var addNode = this.FlowEditor.CreateNodeView(tool);
                    //    //FlowEditView.AddFlowNode(addNode, parentId, childId);
                    //    parentId = node.Id;
                    //}
                    this.FlowEditor.RefreshLayout();

                }
            }
            _moveToolView = null;

            // Handle tool up logic here
            //if(sender is DockPanel panel)
            //{

            //}

        }

        private void ToolListView_ToolDownHandler(object sender, MouseButtonEventArgs args)
        {
            // Handle tool up logic here
        }

    }
}
