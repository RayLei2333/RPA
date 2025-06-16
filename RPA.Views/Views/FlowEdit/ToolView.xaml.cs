using RPA.Controls.FlowEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RPA.Views.Views.FlowEdit
{
    /// <summary>
    /// ToolView.xaml 的交互逻辑
    /// </summary>
    public partial class ToolView : UserControl
    {
        public static readonly DependencyProperty ShowNameProperty =
         DependencyProperty.Register(nameof(ShowName), typeof(string), typeof(ToolView));

        public static readonly DependencyProperty ShowIconProperty =
            DependencyProperty.Register(nameof(ShowIcon), typeof(string), typeof(ToolView));


        public string ShowName
        {
            get => (string)GetValue(ShowNameProperty);
            set => SetValue(ShowNameProperty, value);
        }

        public string ShowIcon
        {
            get => (string)GetValue(ShowIconProperty);
            set => SetValue(ShowIconProperty, value);
        }

        public ToolView()
        {
            InitializeComponent();
        }

        public ToolView Clone()
        {
            ToolView view = new ToolView();
            view.ShowName = this.ShowName;
            view.ShowIcon = this.ShowIcon;
            return view;
        }

    }
}
