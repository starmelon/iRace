using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using RaceManagerTool.Tools;
using RaceManagerTool.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RaceManagerTool.ViewModels
{
    class MainWinViewModel : BindableBase
    {
     
        public DelegateCommand<MainWindow> SetGameInfoCommand { get; set; }

        private GameModeBaseViewModel gameViewModel;
        public GameModeBaseViewModel GameViewModel
        {
            get
            {
                return gameViewModel;
            }

            set
            {
                gameViewModel = value;
                this.OnPropertyChanged("GameViewModel");
            }
        }

        private bool canControControls = false;
        public bool CanControControls
        {
            get
            {
                return canControControls;
            }

            set
            {
                canControControls = value;
                this.OnPropertyChanged("CanControControls");
            }
        }

        


        public MainWinViewModel()
        {

            this.SetGameInfoCommand = new DelegateCommand<MainWindow>(new Action<MainWindow>(this.SetGameInfoCommandExecute));
            

            //订阅更新的消息
            Messenger.Default.Register<GameSetting>(this, new Action<GameSetting>(this.setGameModeViewModel), ViewModelContext.SetGameType);
            Messenger.Default.Register(this, new Action(this.removeGameModeViewModel), ViewModelContext.CleanGame);

            if (GameService.GetInstance().Games.Count != 0)
            {
                setGameModeViewModel(GameService.GetInstance().Game.GameSetting);

            }
            //GameViewModel = new SwissModeViewModel();
        }

        private void removeGameModeViewModel()
        {
            GameViewModel = null;
            CanControControls = false;
            GC.Collect();
        }

        private void setGameModeViewModel(GameSetting game)
        {

            switch (game.GameType)
            {
                case "淘汰赛":
                    GameViewModel = new EliminationModeViewModel();
                    CanControControls = true;
                    break;
                case "瑞士轮":
                    GameViewModel = new SwissModeViewModel();
                    CanControControls = true;
                    break;
                default:
                    
                    break;
            }
        }



        #region SetGameInfoButton

        /// <summary>
        /// 设置比赛信息
        /// </summary>
        /// <param name="win"></param>
        private void SetGameInfoCommandExecute(MainWindow win)
        {

            GameInfoWindow gameInfoWin = GameInfoWindow.GetInstance();
            gameInfoWin.Owner = win;
            gameInfoWin.ShowDialog();

        }

        #endregion

    }
}
