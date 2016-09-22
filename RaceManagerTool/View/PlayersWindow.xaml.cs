using GongSolutions.Wpf.DragDrop;
using RaceManagerTool.ViewModels;
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
using System.Windows.Shapes;

namespace RaceManagerTool.View
{
    /// <summary>
    /// PlayersWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayersWindow : Window
    {
        private static readonly PlayersWindow instance = new PlayersWindow();

        private PlayersWindow()
        {
            InitializeComponent();
            this.DataContext = new PlayersWinViewModel();
        }

        public static PlayersWindow GetInstance()
        {
            return instance;
            
        }

        /// <summary>
        /// 重写OnClosing事件 解决窗口关闭不能再开的bug。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


    }
}
