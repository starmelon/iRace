using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Services
{
    public class AppService
    {
        public static void check()
        {
            String dataPath = Path.Combine(Environment.CurrentDirectory, @"Data");
            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
            }

            string hostFilePath = Path.Combine(dataPath, @"Hosts.xml");
            if (File.Exists(hostFilePath) == false)
            {
                 
                //Uri uri = new Uri("pack://application:,,,/Data/Hosts.xml");
                
                
                //File.Copy(uri.AbsolutePath, hostFilePath);
            }

            string ResultFilePath = Path.Combine(dataPath, @"Results.xml");
            if (File.Exists(ResultFilePath) == false)
            {

            }

            String gamePath = Path.Combine(Environment.CurrentDirectory, @"Game");
            if (Directory.Exists(gamePath) == false)
            {
                Directory.CreateDirectory(gamePath);
            }

            String gameTempPath = Path.Combine(gamePath, @"Temp");
            if (Directory.Exists(gameTempPath) == false)
            {
                Directory.CreateDirectory(gameTempPath);
            }

            String gameFnishPath = Path.Combine(gamePath, @"Finish");
            if (Directory.Exists(gameFnishPath) == false)
            {
                Directory.CreateDirectory(gameFnishPath);
            }
        }
    }
}
