using Prism.Mvvm;
using RaceManagerTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.ViewModels
{
    class DeadPlayerViewModel:BindableBase
    {
        public Player Player { get; set; }

        private bool canSelectRelive;
        public bool CanSelectRelive
        {
            get { return canSelectRelive; }
            set
            {
                canSelectRelive = value;
                OnPropertyChanged("CanSelectRelive");
            }
        }

        private bool canReLive;
        public bool CanReLive
        {
            get { return canReLive; }
            set
            {
                canReLive = value;
                OnPropertyChanged("CanReLive");
            }
        }




        public DeadPlayerViewModel()
        {
            CanSelectRelive = true;
            CanReLive = false;

        }
    }
}
