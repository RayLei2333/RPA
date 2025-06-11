using RPA.Views.FlowEditor;
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

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private MainViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            this.DataContext = _viewModel;
            //FlowNodes = new ObservableCollection<FlowNode>();
            //this.DataContext = this;
        }

      


        private void addRect_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.FlowNodes.Add(new FlowNode()
            {
                IsDraggable = true,
                ShapeType = ShapeType.Rectangle,
                BorderRadius = 8,
                EditorParent = this.editor,
                Name = $"形状{(_viewModel.FlowNodes.Count + 1)}"
            });
        }
    }
}