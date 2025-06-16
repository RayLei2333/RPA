using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA.Controls.FlowEditor
{
    /// <summary>
    /// WorkflowItem.xaml 的交互逻辑
    /// </summary>
    public partial class WorkflowItem : UserControl
    {

        public static readonly DependencyProperty SelectedProperty =
            DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(WorkflowItem));

        public static readonly DependencyProperty ShapeTypeProperty =
            DependencyProperty.Register(nameof(ShapeType), typeof(ShapeType), typeof(WorkflowItem), new PropertyMetadata(ShapeType.Rectangle));

        public static readonly DependencyProperty FlowNodeTypeProperty =
            DependencyProperty.Register(nameof(FlowNodeType), typeof(FlowNodeType), typeof(WorkflowItem));

        public ShapeType ShapeType
        {
            get => (ShapeType)GetValue(ShapeTypeProperty);
            set => SetValue(ShapeTypeProperty, value);
        }

        public FlowNodeType FlowNodeType
        {
            get => (FlowNodeType)GetValue(FlowNodeTypeProperty);
            set => SetValue(FlowNodeTypeProperty, value);
        }


        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }


        public FlowNode FlowNode { get; set; }

        public List<ArrowView> ArrowViews { get; set; }

        
        public WorkflowItem()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void SetNode(FlowNode node)
        {
            FlowNode = node;
            this.textName.Text = node.Name;
        }

        public void AddArrow(ArrowView arrow)
        {
            if (ArrowViews == null)
            {
                ArrowViews = new List<ArrowView>();
            }
            ArrowViews.Add(arrow);
        }

        public bool Contain(Point point)
        {
            Rect rect = new Rect(Canvas.GetLeft(this), Canvas.GetTop(this), this.ActualWidth, this.ActualHeight);
            return rect.Contains(point);
        }
    }
}
