using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Models
{
    [Serializable]
    public class Host:BindableBase
    {
        public string QQ { get; set; }
        public string Name { get; set; }
        public string Introduction { get; set; }
    }
}
