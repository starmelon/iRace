using RaceManagerTool.Services;
using RaceManagerTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GongSolutions.Wpf.DragDrop;

namespace RaceManagerTool
{



    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;
            this.DataContext = new MainWinViewModel();
        }

        private void scheduleItemsDataGrid_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {

            if (e.AddedCells.Count == 0) return;
            var currentCell = e.AddedCells[0];
            string header = (string)currentCell.Column.Header;

            if (currentCell.Column == dg_groups.Columns[3])
            {
                dg_groups.BeginEdit();

            }
        }




        #region 暂未用到



        ////
        // // 使 DataGrid 的单元格单击事件直接变为编辑事件
        // // SINGLE CLICK EDITING
        // //
        // private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        // {
        //     DataGridCell cell = sender as DataGridCell;
        //     if (cell != null && !cell.IsEditing && !cell.IsReadOnly)
        //     {
        //         if (!cell.IsFocused)
        //         {
        //             cell.Focus();
        //         }
        //         DataGrid dataGrid = FindVisualParent<DataGrid>(cell);
        //         if (dataGrid != null)
        //         {
        //             if (dataGrid.SelectionUnit != DataGridSelectionUnit.FullRow)
        //             {
        //                 if (!cell.IsSelected) 
        //                 {
        //                     cell.IsSelected = true;

        //                     Console.WriteLine(cell.ToString());

        //                 }

        //             }
        //             else
        //             {
        //                 DataGridRow row = FindVisualParent<DataGridRow>(cell);
        //                 if (row != null && !row.IsSelected)
        //                 {
        //                     row.IsSelected = true;
        //                 }
        //             }
        //         }
        //     }
        // }

        // static T FindVisualParent<T>(UIElement element) where T : UIElement
        // {
        //     UIElement parent = element;
        //     while (parent != null)
        //     {
        //         T correctlyTyped = parent as T;
        //         if (correctlyTyped != null)
        //         {
        //             return correctlyTyped;
        //         }

        //         parent = VisualTreeHelper.GetParent(parent) as UIElement;
        //     }
        //     return null;
        // }

        #endregion

        private void mainwin_Closed(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }
}
