using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRace.Dao
{
    class ZipDao
    {
        public static void ZipGame(string packname,string packPath,string savePath)
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.Always;
                zip.AlternateEncoding = Encoding.UTF8;
                //zip.AddDirectoryByName(@"\123\");
                zip.AddDirectory(@packPath, packname);
                
                zip.Save(@savePath + ".zip");
            }
        }
    }
}
