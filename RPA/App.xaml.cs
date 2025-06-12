using Framework.Utils;
using System.Configuration;
using System.Data;
using System.Windows;

namespace RPA
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var info = System.Threading.Thread.CurrentThread.CurrentUICulture;
            //string lang = info.IetfLanguageTag;
        }
    }

}
