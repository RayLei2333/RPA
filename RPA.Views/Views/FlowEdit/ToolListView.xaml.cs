using RPA.Views.Models.FlowEdit;
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

        public delegate void ToolSelectDownHandler(object sender, MouseButtonEventArgs args);

        public ToolSelectMoveHandler ToolMoveHandler;

        public ToolSelectUpHandler ToolUpHandler;

        public ToolSelectDownHandler ToolDownHandler;

        private bool MousePress = false;

        private List<ToolGroupModel> _toolGroup;

        public List<ToolGroupModel> ToolGroup { get; set; }

        private string _lang;
        public ToolListView()
        {
            InitializeComponent();
            this.DataContext = this;
            var info = System.Threading.Thread.CurrentThread.CurrentUICulture;
            _lang = info.ToString();
            InitToolbox();
        }

        public void InitToolbox()
        {
            string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"res/Config/ToolList_{_lang}.json");
            var list = JsonSerializer.Deserialize<List<ToolGroupModel>>(File.ReadAllText(configPath));
            if (list == null || !list.Any())
                return;

            list = list.Where(t => t.Enable).ToList();
            foreach (var item in list)
            {
                item.Tools = item.Tools.Where(t => t.Enable).ToList();
            }
            ToolGroup = list;
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
