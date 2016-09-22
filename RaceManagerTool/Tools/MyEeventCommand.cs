using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace RaceManagerTool.Tools
{
    class MyEeventCommand:TriggerAction<DependencyObject>
    {
        ///// <summary>
        ///// 事件要绑定的命令
        ///// </summary>
        //public ICommand Command
        //{
        //    get { return (ICommand)GetValue(CommmandProperty); }
        //    set { SetValue(CommmandProperty, value); }
        //}

        //public static readonly DependencyProperty CommmandProperty =
        //    DependencyProperty.Register("Command", typeof(ICommand), typeof(MyEeventCommand), new PropertyMetadata(null));

        //public object CommmandParameter
        //{
        //    get { return (object)GetValue(CommandParameterProperty); }
        //    set { SetValue(CommandParameterProperty, value); }
        //}

        //public static readonly DependencyProperty CommandParameterProperty =
        //    DependencyProperty.Register("CommandParameter",typeof(object),typeof(MyEeventCommand),new PropertyMetadata(null)());



        protected override void Invoke(object parameter)
        {
        //    if (CommmandParameter != null)
        //    {
        //        parameter = CommmandParameter;
        //    }
        //    var cmd = Command;
        //    if (cmd != null)
        //    {
        //        cmd.Execute(parameter);
        //    }
        }

       
    }
}
