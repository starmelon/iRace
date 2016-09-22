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
    /// ReLiveWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReLiveWindow : Window
    {
        public ReLiveWindow()
        {
            InitializeComponent();
            ReLiveWinViewModel vm = new ReLiveWinViewModel();
            vm.ClosingRequest += (sender, e) => this.Close();
            this.DataContext = vm;
        }
    }
}
