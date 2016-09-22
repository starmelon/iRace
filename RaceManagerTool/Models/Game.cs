using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Models
{
    [Serializable]
    public class Game:BindableBase
    {
        public ObservableCollection<Player> Players { get; set; }
        public GameSetting GameSetting { get; set; }
        public ObservableCollection<Turn>  Turns{ get; set; }

        public Game()
        {
            Players = new ObservableCollection<Player>();
            GameSetting = new GameSetting();
            Turns = new ObservableCollection<Turn>();
        }
    }
}
