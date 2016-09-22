using RaceManagerTool.Models;
using RaceManagerTool.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;


namespace RaceManagerTool.Services
{
    [System.Xml.Serialization.XmlInclude(typeof(Player))]
    public sealed class PlayerService
    {
        private static readonly PlayerService instance = new PlayerService();

   
        
        //public ObservableCollection<Player> Players { get; set; }

        
        
        private PlayerService()
        {
            //Players = new ObservableCollection<Player>();
        }

        public static PlayerService GetInstance()
        {
            return instance;
        }






        

        

        

        



        

        

        

         
    }

}
