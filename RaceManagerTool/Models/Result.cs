using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Models
{
    public class Result:BindableBase
    {
        public int Index { get; set; }

        public const int WIN = 0;
        public const int TIE = 1;
        public const int LOSE = 2;

        public int Win { get; set; }
        public int Tie { get; set; }
        public int Lose { get; set; }
        public string Define { get; set; }
        public int PointLeft { get; set; }
        public int PointRight { get; set; }
    }
}
