using RaceManagerTool.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRace.Dao
{
    class TextDao
    {
        public static void OutPutTurns(List<Turn> turns)
        {

        }

        public static void OutPutTurn(string path,StringBuilder strb)
        {
            FileStream fs = new FileStream(@path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            sw.Write(strb);
            sw.Close();
            fs.Close();
        }

    }
}
