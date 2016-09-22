using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using RaceManagerTool.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.ViewModels
{
    class PlayersWinViewModel : BindableBase
    {
        public List<Player> players { get; set; }
        public List<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                players = value;
                this.OnPropertyChanged("Players");
            }
        }
        //public ObservableCollection<Player> Players { get; set; }

        public DelegateCommand OutPutRankCommand { get; set; }
        public DelegateCommand ActivatedWinCommand { get; set; }

        public PlayersWinViewModel()
        {
            Players = new List<Player>(GameService.GetInstance().Game.Players);
            this.OutPutRankCommand = new DelegateCommand(new Action(this.OutPutRankCommandExecute));
            this.ActivatedWinCommand = new DelegateCommand(ActivatedWinCommandExecute);

            Messenger.Default.Register<List<Player>>(this, new Action<List<Player>>(this.refreshRank), ViewModelContext.UpdataRank);
            //new Action(this.ActivatedWinCommandExecute)
        }

        private void refreshRank(List<Player> players)
        {
            GameService.GetInstance().CountAllPlayersPointsByLastTurn();
            Players = players;
        }

        /// <summary>
        /// 窗体激活时执行的命令
        /// </summary>
        private void ActivatedWinCommandExecute()
        {
            //PlayerService.GetInstance().CountAllPlayersPointsByLastTurn();
        }

        private void OutPutRankCommandExecute()
        {
            GameService.GetInstance().OutPutRank();
        }

        
    }
}
