using RaceManagerTool.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RaceManagerTool
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        static void Main()
        {
            App app = new App();
            app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

            AppService.check();
            GameService.GetInstance().init();

            app.Run();
        }
    }
}
