using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RaceManagerTool.ViewModels
{
    abstract class GameModeBaseViewModel : BindableBase
    {
        /// <summary>
        /// 可设置的比赛结果集合
        /// </summary>
        public List<Result> Results
        {
            get { return results; }
            set { results = value; }
        }

        public GameSetting Game { get; set; }

        /// <summary>
        /// 轮次Combox集合
        /// </summary>
        public ObservableCollection<string> ListTurns { get; set; }

        private int selectTurn;
        /// <summary>
        /// 当前选择的回合Index
        /// </summary>
        public int SelectTurn
        {
            get { return selectTurn; }
            set
            {
                selectTurn = value;
                OnPropertyChanged("SelectTurn");
            }
        }

        
        private List<Result> results;


        /// <summary>
        /// Combox中的选择项
        /// </summary>
        public Group selectItem;
        public Group SelectItem
        {
            get { return selectItem; }
            set
            {
                selectItem = value;
                OnPropertyChanged("SelectItem");
            }
        }
        /// <summary>
        /// Combox中的被编辑项项
        /// </summary>
        protected Group EditedItem { get; set; }

        private Turn turn;
        public Turn Turn
        {
            get { return turn; }
            set
            {
                turn = value;
                OnPropertyChanged("Turn");
            }
        }



        public DelegateCommand StartGameCommand { get; set; }

        public DelegateCommand SetResultCommand { get; set; }
        public DelegateCommand CleanResultsCommand { get; set; }
        public DelegateCommand CellEditEndingCommand { get; set; }
        public DelegateCommand CheckPlayersCommand { get; set; }


        private DelegateCommand startNextTurnCommand;
        public DelegateCommand StartNextTurnCommand
        {
            get { return startNextTurnCommand; }
            set
            {
                startNextTurnCommand = value;
                OnPropertyChanged("StartNextTurnCommand");
            }
        }

        public DelegateCommand RomoveTurnCommand { get; set; }


        public DelegateCommand resetTurnCommand;
        public DelegateCommand ResetTurnCommand
        {
            get { return resetTurnCommand; }
            set
            {
                resetTurnCommand = value;
                OnPropertyChanged("ResetTurnCommand");
            }
        }

        public DelegateCommand ResetSwissTurnCommand { get; set; }
        public DelegateCommand ResetEliminationTurnCommand { get; set; }

        public DelegateCommand ReRangeGroupCommand { get; set; }
        public DelegateCommand SetOverTimeCommand { get; set; }
        public DelegateCommand SelectTurnCommand { get; set; }

        public DelegateCommand ReLiveCommand { get; set; }
        public DelegateCommand OutPutGroupCommand { get; set; }
        public DelegateCommand EndToZipGameCommand { get; set; }
        

        public GameModeBaseViewModel()
        {
            SelectTurn = -1;

            ListTurns = new ObservableCollection<string>();

            this.StartGameCommand = new DelegateCommand(new Action(this.StartGameCommandExecute), new Func<bool>(this.canStartGameCommandExecute));

            //this.SetGameInfoCommand = new DelegateCommand(new Action(this.SetGameInfoCommandExecute));
            this.SetResultCommand = new DelegateCommand(new Action(this.SetResultCommandExecute));
            this.CleanResultsCommand = new DelegateCommand(new Action(this.CleanResultsCommandExecute), new Func<bool>(this.canCleanResultsCommand));
            this.CellEditEndingCommand = new DelegateCommand(new Action(this.CellEditEndingCommandExecute));
            this.CheckPlayersCommand = new DelegateCommand(new Action(this.CheckPlayersCommandExecute), new Func<bool>(this.canCheckPlayersCommandExecute));
            this.StartNextTurnCommand = new DelegateCommand(new Action(this.StartNextTurnCommandExecute), new Func<bool>(this.canStartNextTurnCommandExecute));
            this.ResetTurnCommand = new DelegateCommand(new Action(this.ResetTurnCommandExecute), new Func<bool>(this.canResetTurnCommandExecute));
            this.RomoveTurnCommand = new DelegateCommand(new Action(this.RomoveTurnCommandExecute), new Func<bool>(this.canRomoveTurnCommandExecute));
            this.ReRangeGroupCommand = new DelegateCommand(new Action(this.ReRangeGroupCommandExecute), new Func<bool>(this.canReRangeGroupCommandExecute));
            this.SetOverTimeCommand = new DelegateCommand(new Action(this.SetOverTimeCommandExecute), new Func<bool>(this.canSetOverTimeCommandExecute));
            this.ReLiveCommand = new DelegateCommand(new Action(this.ReLiveCommandExecute), new Func<bool>(this.canReLiveCommandExecute));
            this.OutPutGroupCommand = new DelegateCommand(new Action(this.OutPutGroupCommandExecute),new Func<bool>(CanOutPutGroupCommandExecute));
            this.SelectTurnCommand = new DelegateCommand(new Action(this.SelectTurnCommandExecute));
            this.EndToZipGameCommand = new DelegateCommand(new Action(this.EndToZipGameCommandExecute),new Func<bool>(CanEndToZipGameCommandExecute));


            Results = GameService.GetInstance().Results;
            Turn = GameService.GetInstance().getLastTurn();
            Game = GameService.GetInstance().Game.GameSetting;

            int turnsum = GameService.GetInstance().Game.Turns.Count;

            for (int i = 1; i <= turnsum; i++)
            {
                ListTurns.Add(getTurnName(i));
            }
            SelectTurn = turnsum - 1;
        }

        /// <summary>
        /// 预判能否执行EndToZipGameCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanEndToZipGameCommandExecute()
        {
            if (GameService.GetInstance().Game == null)
            {
                return false;
            }

            if (GameService.GetInstance().Game.Turns.Count != GameService.GetInstance().Game.GameSetting.Turns)
            {
                return false;
            }

            if (GameService.GetInstance().isAllResultsSet() == false)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 结束并压缩比赛文件的命令
        /// </summary>
        private void EndToZipGameCommandExecute()
        {
            GameService.GetInstance().EndToZipGame();
        }

        #region 抽象方法组

        protected abstract bool canStartGameCommandExecute();
        protected abstract void StartGameCommandExecute();
        protected abstract void SetResultCommandExecute();
        protected abstract bool canCleanResultsCommand();
        protected abstract void CleanResultsCommandExecute();
        protected abstract void CellEditEndingCommandExecute();
        protected abstract bool canCheckPlayersCommandExecute();
        protected abstract void CheckPlayersCommandExecute();

        protected abstract bool canStartNextTurnCommandExecute();
        protected abstract void StartNextTurnCommandExecute();

        protected abstract bool canResetTurnCommandExecute();
        protected abstract void ResetTurnCommandExecute();



        protected abstract bool canRomoveTurnCommandExecute();
        protected abstract void RomoveTurnCommandExecute();

        protected abstract bool canReRangeGroupCommandExecute();
        protected abstract void ReRangeGroupCommandExecute();

        protected abstract bool canSetOverTimeCommandExecute();
        protected abstract void SetOverTimeCommandExecute();

        protected abstract bool canReLiveCommandExecute();
        protected abstract void ReLiveCommandExecute();

        protected abstract string getTurnName(int turnIndex);

        #endregion

        #region 虚方法组

        /// <summary>
        /// 执行切换查看轮次信息
        /// </summary>
        protected virtual void SelectTurnCommandExecute()
        {
            if (SelectTurn == -1 || GameService.GetInstance().Game.Turns.Count == 0)
            {
                return;
            }
            Turn = GameService.GetInstance().Game.Turns[SelectTurn];

            checkAllCommand();
            //RomoveTurnCommand.RaiseCanExecuteChanged();
            //StartNextTurnCommand.RaiseCanExecuteChanged();
            //ReLiveCommand.RaiseCanExecuteChanged();
            //ResetTurnCommand.RaiseCanExecuteChanged();
            //SetOverTimeCommand.RaiseCanExecuteChanged();
        }

        protected virtual bool CanOutPutGroupCommandExecute()
        {
            if (GameService.GetInstance().Game.Turns.Count < 1)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 输出当前分组战况信息
        /// </summary>
        protected virtual void OutPutGroupCommandExecute()
        {
            GameService.GetInstance().OutPutGroupToClipBoard(SelectTurn);
            MessageBox.Show("已复制到剪贴板");
        }

        #endregion

        /// <summary>
        /// 更新所有命令的可用状态
        /// </summary>
        protected void checkAllCommand()
        {
            this.StartGameCommand.RaiseCanExecuteChanged();
            this.SetResultCommand.RaiseCanExecuteChanged();
            this.CleanResultsCommand.RaiseCanExecuteChanged();
            this.CellEditEndingCommand.RaiseCanExecuteChanged();
            this.CheckPlayersCommand.RaiseCanExecuteChanged();
            this.StartNextTurnCommand.RaiseCanExecuteChanged();
            this.ResetTurnCommand.RaiseCanExecuteChanged();
            this.RomoveTurnCommand.RaiseCanExecuteChanged();
            this.ReRangeGroupCommand.RaiseCanExecuteChanged();
            this.SetOverTimeCommand.RaiseCanExecuteChanged();
            this.ReLiveCommand.RaiseCanExecuteChanged();
            this.OutPutGroupCommand.RaiseCanExecuteChanged();
            this.SelectTurnCommand.RaiseCanExecuteChanged();
            this.EndToZipGameCommand.RaiseCanExecuteChanged();
        }
    }
}
