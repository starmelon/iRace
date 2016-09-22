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
    class ReLiveWinViewModel:BindableBase
    {
        public ObservableCollection<DeadPlayerViewModel> LosePlayers { get; set; }

        public DelegateCommand ReliveCommand { get; set; }
        public DelegateCommand OutputSelectPlayers { get; set; }
        public DelegateCommand SelectReliveCommand { get; set; }

        public int ReliveNumMax { get; set; }

        private int reliveNum;
        public int ReliveNum
        {
            get { return reliveNum; }
            set
            {
                reliveNum = value;
                this.OnPropertyChanged("ReliveNum");
            }
        }

        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            if (this.ClosingRequest != null)
            {
                this.ClosingRequest(this, EventArgs.Empty);
            }
        }


        public ReLiveWinViewModel()
        {
            LosePlayers = new ObservableCollection<DeadPlayerViewModel>();
            List<Player> losers = GameService.GetInstance().getLosePlayers();

            foreach (var l in losers)
            {
                DeadPlayerViewModel dpvm = new DeadPlayerViewModel();
                dpvm.Player = l;
                LosePlayers.Add(dpvm);
            }

            this.ReliveCommand = new DelegateCommand(new Action(this.ReliveCommandExecute),new Func<bool>(this.CanReliveCommandExecute));
            this.OutputSelectPlayers = new DelegateCommand(new Action(this.OutputSelectPlayersExecute),new Func<bool>(this.CanOutputSelectPlayersExecute));
            this.SelectReliveCommand = new DelegateCommand(new Action(this.SelectReliveCommandExecute));

            ReliveNumMax = GameService.GetInstance().getRelvieNumMax();
        }


        /// <summary>
        /// 预判能否执行CanOutputSelectPlayersExecute
        /// </summary>
        /// <returns></returns>
        private bool CanOutputSelectPlayersExecute()
        {
            return true;
        }

        /// <summary>
        /// 复制选择的复活名单
        /// </summary>
        private void OutputSelectPlayersExecute()
        {

            List<Player> canRelivePlayer = new List<Player>();
            foreach (var p in LosePlayers)
            {
                if (p.CanReLive)
                {
                    canRelivePlayer.Add(p.Player);
                }
            }
            GameService.GetInstance().OutPutSelectReLivePlayers(canRelivePlayer);
        }

        private void SelectReliveCommandExecute()
        {
            ReliveNum = this.LosePlayers.Count(i => i.CanReLive == true);

        }


        /// <summary>
        /// 预判能否执行ReliveCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanReliveCommandExecute()
        {
            return true;
        }

        /// <summary>
        /// 复活命令
        /// </summary>
        private void ReliveCommandExecute()
        {
            List<Player> reLivePlayers = new List<Player>();
            foreach (var lpvm in LosePlayers)
            {
                if (lpvm.CanReLive)
                {
                    reLivePlayers.Add(lpvm.Player);
                }
            }
            GameService.GetInstance().relive(reLivePlayers);
            Messenger.Default.Send(GameService.GetInstance().getLastTurn(), ViewModelContext.ShowTurns);
            OnClosingRequest();
        }
    }
}
