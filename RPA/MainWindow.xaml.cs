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
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
            //editor = new WorkFlowEditor();

            //this.grid.Children.Add(editor);
            //FlowNodes = new ObservableCollection<FlowNode>();
            //this.DataContext = this;
        }




        private void addRect_Click(object sender, RoutedEventArgs e)
        {
            //var node1 = new FlowNode()
            //{
            //    Id = "1",
            //    Name = "1",
            //    ParentId = null,
            //    ChildId = new List<FlowNode>()
            //    {
                    
            //    }
            //};

            //var node2 = new FlowNode()
            //{
            //    Id = "2",
            //    Name = "2",
            //    ParentId = node1,
            //    ChildId = new List<FlowNode>()
            //    {

            //    }
            //};

            //var node3 = new FlowNode()
            //{
            //    Id = "3",
            //    Name = "3",
            //    ParentId = node1,
            //    ChildId = new List<FlowNode>()
            //    {

            //    }
            //};

            //node1.ChildId.Add(node2);
            //node1.ChildId.Add(node3);

            //List<FlowNode> list = new List<FlowNode>()
            //{

            //};

            //editor.SetFlowNodes(new List<FlowNode>() { node1 });
            //this.AddChild(editor);

            //_viewModel.FlowNodes.Add(new FlowNode()
            //{
            //    IsDraggable = true,
            //    ShapeType = ShapeType.Rectangle,
            //    BorderRadius = 8,
            //    EditorParent = this.editor,
            //    Name = $"形状{(_viewModel.FlowNodes.Count + 1)}"
            //});
        }

        private void openEdit_Click(object sender, RoutedEventArgs e)
        {
            FlowEditWindow flowEdit = new FlowEditWindow();
            flowEdit.Show();
        }
    }
}