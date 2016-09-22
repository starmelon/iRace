using RaceManagerTool.Services;
using RaceManagerTool.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using GongSolutions.Wpf.DragDrop;

namespace RaceManagerTool
{
    

    /// <summary>
    /// GameInfo.xaml 的交互逻辑
    /// </summary>
    public partial class GameInfoWindow : Window
    {

        private static readonly GameInfoWindow instance = new GameInfoWindow();


        private GameInfoWindow()
        {
            InitializeComponent();

            GameInfoWinViewModel vm = new GameInfoWinViewModel();
            vm.ClosingRequest += (sender, e) => this.Close();

            this.DataContext = vm;

            
        }

        public static GameInfoWindow GetInstance()
        {
            
            return instance;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Hide();
            e.Cancel = true;
        }

    }
}
