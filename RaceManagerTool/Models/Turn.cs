using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Models
{
    [Serializable]
    [System.Xml.Serialization.XmlInclude(typeof(Group))]
    public class Turn
    {
        public int TurnIndex { get; set; }


        public ObservableCollection<Group> Groups { get; set; }

        public Turn()
        {
            Groups = new ObservableCollection<Group>() { };
        }
    }
}
