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

namespace RPA.Views.Views.FlowEdit
{
    /// <summary>
    /// ToolView.xaml 的交互逻辑
    /// </summary>
    public partial class ToolView : UserControl
    {
        private string _toolType;

        public ToolView()
        {
            InitializeComponent();
        }

        public void SetTool(string icon, string name, string type)
        {
            this.imgToolIcon.Text = icon;
            this.tbkToolName.Text = name;
            _toolType = type;
        }

        public ToolView Clone()
        {
            ToolView toolView = new ToolView();
            toolView.SetTool(imgToolIcon.Text, tbkToolName.Text, _toolType);
            return toolView;
        }
    }
}
