using RPA.Views.ViewModels.FlowEdit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
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
    /// ToolListView.xaml 的交互逻辑
    /// </summary>
    public partial class ToolListView : UserControl
    {

        public delegate void ToolSelectMoveHandler(object sender, MouseEventArgs args);

        public delegate void ToolSelectUpHandler(object sender, MouseButtonEventArgs args);

        public ToolSelectMoveHandler ToolMoveHandler;

        public ToolSelectUpHandler ToolUpHandler;

        private bool MousePress = false;

        private List<ToolGroupModel> _toolGroup;
        private string _lang;
        public ToolListView()
        {
            InitializeComponent();
            var info = System.Threading.Thread.CurrentThread.CurrentUICulture;
            _lang = info.ToString();
            //_toolGroup = new List<ToolGroupModel>();
            InitToolbox();
        }

        public void InitToolbox()
        {
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"res/Config/ToolList_{_lang}.json");
            _toolGroup = JsonSerializer.Deserialize<List<ToolGroupModel>>(File.ReadAllText(configPath));

            if (_toolGroup == null || !_toolGroup.Any())
                return;
            var list = _toolGroup.Where(t => t.Enable).ToList();
            foreach (var item in list)
            {
               
                Expander expander = new Expander();
                var style = Application.Current.FindResource("ExpanderStyle");//this.Resources["ExpanderStyle"];
                expander.SetValue(Expander.StyleProperty, style);
                Label label = new Label();
                label.Content = item.Name;
                label.FontSize = 14;
                label.Foreground = new SolidColorBrush(Colors.White);
                expander.Header = label;
                expander.Margin = new Thickness(12, 0, 0, 0);
                if(item.Tools != null && item.Tools.Any())
                {
                    var tools = item.Tools.Where(t => t.Enable).ToList();
                    int toolCount = tools.Count;
                    var gridLayout = new Grid();
                    int col = 3;
                    int row = toolCount / 3 + (toolCount % 3 == 0 ? 0 : 1);
                    for (int i = 0; i < col; i++)
                    {
                        gridLayout.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    }
                    for (int i = 0; i < row; i++)
                    {
                        gridLayout.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
                    }
                    for (int i = 0; i < toolCount; i++)
                    {
                        var tool = tools[i];
                        ToolView toolView = new ToolView();
                        toolView.SetTool(tool.Icon, tool.Name, tool.Type);
                        gridLayout.Children.Add(toolView);
                        toolView.MouseDown += new MouseButtonEventHandler(ToolViewMouseDown);
                        toolView.MouseMove += new MouseEventHandler(ToolViewMouseMove);
                        toolView.MouseUp += new MouseButtonEventHandler(ToolViewMouseUp);
                        Grid.SetColumn(toolView, i % 3);
                        Grid.SetRow(toolView, i / 3);
                    }
                    expander.Content = gridLayout;

                }
                


                expander.IsExpanded = true;
                expander.Margin = new Thickness(12, 12, 12, 12);
                spToolList.Children.Add(expander);
                spToolList.Background= new SolidColorBrush(Color.FromRgb(38, 38, 38));
            }
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToolViewMouseDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                ToolView view = sender as ToolView;
                if (view != null)
                {
                    view.CaptureMouse();
                    MousePress = true;
                }
            }
        }

        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToolViewMouseMove(object sender, MouseEventArgs args)
        {
            if (MousePress && args.LeftButton == MouseButtonState.Pressed && ToolMoveHandler != null)
            {
                ToolMoveHandler(sender, args);
            }
        }

        /// <summary>
        /// 鼠标释放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ToolViewMouseUp(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton == MouseButton.Left)
            {
                if (ToolUpHandler != null)
                {
                    ToolUpHandler(sender, args);
                }
                ToolView view = sender as ToolView;
                if (view != null)
                {
                    view.ReleaseMouseCapture();
                }
                MousePress = false;
            }
        }
    }
}
