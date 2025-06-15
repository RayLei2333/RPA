using RPA.Controls.FlowEditor;
using RPA.Views.Views.FlowEdit;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window//, INotifyPropertyChanged
    {
        private MainViewModel _viewModel;
        //private WorkFlowEditor editor;
        public MainWindow()
        {
            InitializeComponent();
            //_viewModel = new MainViewModel();
            //this.DataContext = _viewModel;
            //editor = new WorkFlowEditor();

            //this.grid.Children.Add(editor);
            //FlowNodes = new ObservableCollection<FlowNode>();
            //this.DataContext = this;
        }




        private void addRect_Click(object sender, RoutedEventArgs e)
        {
            FlowNode sNode = new FlowNode()
            {
                Id = "start",
                Name = "开始",
                NodeType = FlowNodeType.Start,
                ShapeType = ShapeType.Circle
            };

            FlowNode mNode = new FlowNode()
            {
                Id = "node1",
                Name = "节点1",
                Parent = sNode,
                NodeType = FlowNodeType.Default,
                ShapeType = ShapeType.Diamond
                
            };

            FlowNode eNode = new FlowNode()
            {
                Id = "end",
                Name = "结束",
                Parent = mNode,
                NodeType = FlowNodeType.End,
                ShapeType = ShapeType.Rectangle
            };

            sNode.Childs.Add(mNode);
            mNode.Childs.Add(eNode);

            Dictionary<string, FlowNode> nodes = new Dictionary<string, FlowNode>()
            {
                ["start"] = sNode,
                ["end"] = eNode
            };
            this.editor.SetFlowNodes(nodes);
            this.editor.InitNodeView(0, 0, this.editor.NodeStream.Width);
        }

        private void openEdit_Click(object sender, RoutedEventArgs e)
        {
            FlowEditWindow flowEdit = new FlowEditWindow();
            flowEdit.Show();
        }
    }
}