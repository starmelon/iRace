﻿using RaceManagerTool.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
            app.StartupUri = new Uri(@"View\MainWindow.xaml", UriKind.Relative);

            AppService.check();
            GameService.GetInstance().init();

            app.Run();
        }


        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            var executingAssemblyName = executingAssembly.GetName();
            var resName = executingAssemblyName.Name + ".resources";

            AssemblyName assemblyName = new AssemblyName(args.Name); string path = "";
            if (resName == assemblyName.Name)
            {
                path = executingAssemblyName.Name + ".g.resources"; ;
            }
            else
            {
                path = assemblyName.Name + ".dll";
                if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false)
                {
                    path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
                }
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null)
                    return null;

                byte[] assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }
    }
}
