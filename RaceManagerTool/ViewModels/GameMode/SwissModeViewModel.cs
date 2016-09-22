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
    class SwissModeViewModel : GameModeBaseViewModel
    {


        //public ObservableCollection<string> ListTurns { get; set; }

        //private int selectTurn;

        //public int SelectTurn
        //{
        //    get { return selectTurn; }
        //    set
        //    {
        //        selectTurn = value;
        //        OnPropertyChanged("SelectTurn");
        //    }
        //}
        

        //private List<Result> results;

        //public List<Result> Results
        //{
        //    get { return results; }
        //    set { results = value; }
        //}


        

        //public Group SelectItem
        //{
        //    get { return selectItem; }
        //    set
        //    {
        //        selectItem = value;
        //        OnPropertyChanged("SelectItem");
        //    }
        //}

        
        

        //public DelegateCommand StartGameCommand { get; set; }
        //public DelegateCommand<MainWindow> StartEliminationGameCommand { get; set; }
        //public DelegateCommand<MainWindow> SetGameInfoCommand { get; set; }
        //public DelegateCommand<ComboBox> SetResultCommand { get; set; }
        //public DelegateCommand CleanResultsCommand { get; set; }
        //public DelegateCommand CellEditEndingCommand { get; set; }
        //public DelegateCommand CheckPlayersCommand { get; set; }


        //public DelegateCommand startNextTurnCommand;
        //public DelegateCommand StartNextTurnCommand
        //{
        //    get { return startNextTurnCommand; }
        //    set
        //    {
        //        startNextTurnCommand = value;
        //        OnPropertyChanged("StartNextTurnCommand");
        //    }
        //}




        //public DelegateCommand RomoveTurnCommand { get; set; }


        //public DelegateCommand resetTurnCommand;
        //public DelegateCommand ResetTurnCommand
        //{
        //    get { return resetTurnCommand; }
        //    set
        //    {
        //        resetTurnCommand = value;
        //        OnPropertyChanged("ResetTurnCommand");
        //    }
        //}

        //public DelegateCommand ResetSwissTurnCommand { get; set; }
        //public DelegateCommand ResetEliminationTurnCommand { get; set; }

        //public DelegateCommand ReRangeGroupCommand { get; set; }
        //public DelegateCommand SetOverTimeCommand { get; set; }
        //public DelegateCommand SelectTurnCommand { get; set; }

        //public DelegateCommand ReLiveCommand { get; set; }
        //public DelegateCommand OutPutGroupCommand { get; set; }



        public SwissModeViewModel():base()
        {
            
            LoadResults();
            LoadGame();
            LoadPlayers();
            LoadTurns();

            //订阅轮次更新的消息
            Messenger.Default.Register<Turn>(this, new Action<Turn>(this.showTurn), ViewModelContext.ShowTurns);
 

        }

        

        #region Init

        /// <summary>
        /// 加载玩家信息
        /// </summary>
        private void LoadPlayers()
        {
//PlayerService.GetInstance().loadPlaysFromDisk();

        }


        /// <summary>
        /// 加载游戏设置
        /// </summary>
        private void LoadGame()
        {
            //GameService.GetInstance().LoadGameSettingFromDisk();
            //Game = GameService.GetInstance().Game;
        }

        /// <summary>
        /// 加载轮次信息
        /// </summary>
        private void LoadTurns()
        {


            //GameService.GetInstance().LoadGameInfoFromDisk();
            //if (GameService.GetInstance().Turns.Count < 1)
            //{
            //    return;
            //}


            //int turnsum = GameService.GetInstance().Turns.Count;

            //if (GameService.GetInstance().Game.GameType.Equals("淘汰赛"))
            //{
            //    for (int i = 1; i <= turnsum; i++)
            //    {
            //        ListTurns.Add(getTurnName(i));
            //    }
            //    SelectTurn = turnsum - 1;
            //}
            //else if (GameService.GetInstance().Game.GameType.Equals("瑞士轮"))
            //{
            //    for (int i = 0; i < turnsum; i++)
            //    {
            //        ListTurns.Add(getTurnName(i));
            //    }

            //    SelectTurn = turnsum - 1;
            //}
            
        }

        private void LoadResults()
        {
            Results = GameService.GetInstance().Results;
        }

        #endregion


        #region GameStartButton

        protected override bool canStartGameCommandExecute()
        {
            if (ListTurns.Count != 0)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// 开启比赛
        /// </summary>
        /// <param name="win"></param>
        protected override void StartGameCommandExecute()
        {

            if (GameService.GetInstance().StartNewGame())
            {

                ListTurns.Add(getTurnName(1));
                SelectTurn = 0;


                checkAllCommand();
                GameService.GetInstance().SaveGameInfo2Disk();
                Messenger.Default.Send(ViewModelContext.GameInfoChange);
                //StartNextTurnCommand.RaiseCanExecuteChanged();
                //ReRangeGroupCommand.RaiseCanExecuteChanged();
                //StartGameCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region RemoveTurnButton

        /// <summary>
        /// 预判能否执行RomoveTurnCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canRomoveTurnCommandExecute()
        {
            if (SelectTurn == -1)
            {
                return false;
            }

            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 执行删除当前轮的命令
        /// </summary>
        protected override void RomoveTurnCommandExecute()
        {
            SelectTurn = ListTurns.Count - 2;
            ListTurns.RemoveAt(ListTurns.Count - 1);
            GameService.GetInstance().removeNewestTurn();

            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);

            checkAllCommand();

        }

        #endregion

        #region RestButton

        /// <summary>
        /// 预判是否可执行ResetTurnCommandExecute命令
        /// </summary>
        /// <returns></returns>
        protected override bool canResetTurnCommandExecute()
        {
            //轮次Combox被置空
            if (SelectTurn == -1)
            {
                return false;
            }

            //当前仅存在第一轮
            if (ListTurns.Count == 1)
            {
                return false;
            }

            //不处于当前最新轮
            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 执行重置当前淘汰轮命令
        /// </summary>
        protected override void ResetTurnCommandExecute()
        {
            GameService.GetInstance().RestEliminationTurn();
            Turn = GameService.GetInstance().getLastTurn();

            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);

            checkAllCommand();
        }

        /// <summary>
        /// 执行重置当前瑞士轮命令
        /// </summary>
        private void ResetSwissTurnCommandExecute()
        {
            GameService.GetInstance().RestSwissTurn();
        }

        #endregion

        #region CleanResulsButton

        /// <summary>
        /// 预判能否执行CleanResultsCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canCleanResultsCommand()
        {
            ////瑞士轮中，清空和重置是等价的
            //if (GameService.GetInstance().Game.GameType.Equals("瑞士轮"))
            //{
            //    return false;
            //}

            ////判断选择的是否是当前轮
            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            //判断是否存在已设置结果的卡组
            if (GameService.GetInstance().isExistResultSet() == false)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 清空轮次结果的命令
        /// </summary>
        protected override void CleanResultsCommandExecute()
        {
            if (SelectTurn != ListTurns.Count - 1)
            {
                MessageBox.Show("只允许清空进行中的最后一轮！");
                return;
            }

            GameService.GetInstance().CleanAllResults();

            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);

            checkAllCommand();

        }

        #endregion

        #region SetOverTimeButton

        /// <summary>
        /// 预判能否执行SetOverTimeCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canSetOverTimeCommandExecute()
        {
            if (SelectTurn == -1)
            {
                return false;
            }

            //判断选择的是否是当前轮
            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            //判断是否已设置所有结果
            if (GameService.GetInstance().isAllResultsSet())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 设置超时的执行命令
        /// </summary>
        protected override void SetOverTimeCommandExecute()
        {

            GameService.GetInstance().SetSwissTurnOverTime();


            checkAllCommand();
            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);

        }

        #endregion

        #region NextTurnButton

        /// <summary>
        /// 预判能否执行 StartNextSwissTurnCommandExecute 或 StartNextEliminationTurnCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canStartNextTurnCommandExecute()
        {
            //判断选择的是否是当前轮
            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            //判断是否已设置所有结果
            if (!GameService.GetInstance().isAllResultsSet())
            {
                return false;
            }

            //判断是否是最终轮次
            if (GameService.GetInstance().isFinalTurn())
            {
                return false;
            }

            return true;

        }

        /// <summary>
        /// 开启下一轮瑞士轮
        /// </summary>
        /// <param name="win"></param>
        protected override void StartNextTurnCommandExecute()
        {
            if (GameService.GetInstance().StartNextSwissTurn())
            {
                ListTurns.Add(getTurnName(ListTurns.Count + 1));
                SelectTurn = ListTurns.Count - 1;
                //win.sp_turns.Children.Add(button);
                //win.dg_groups.ItemsSource = null;
                Turn = GameService.GetInstance().getLastTurn();
                //win.dg_groups.ItemsSource = Turns[Convert.ToInt32(button.Content) - 1].Groups;

                checkAllCommand();


            }
            else
            {
                MessageBox.Show("请检查当前轮次所有分组结果是否未填写");
            }
        }

        #endregion

        #region ReliveButton

        /// <summary>
        /// 预判能否执行ReLiveCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canReLiveCommandExecute()
        {
            if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            return GameService.GetInstance().canRelive();
        }

        /// <summary>
        /// 复活处理命令
        /// </summary>
        protected override void ReLiveCommandExecute()
        {
            if (GameService.GetInstance().canRelive() == false)
            {
                MessageBox.Show("请先清空比分");
                return;
            }

            ReLiveWindow rlWin = new ReLiveWindow();
            rlWin.ShowDialog();
            ReLiveCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region ReRangeGroupButton


        /// <summary>
        /// 预判能否执行ReSortCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canReRangeGroupCommandExecute()
        {
            if (SelectTurn == -1)
            {
                return false;
            }

            //判断是否是当前轮
                if (SelectTurn != ListTurns.Count - 1)
            {
                return false;
            }

            //判断是否已存在设置结果分组
            if (GameService.GetInstance().isExistResultSet())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 重新排表
        /// </summary>
        /// <param name="win"></param>
        protected override void ReRangeGroupCommandExecute()
        {
            if (MessageBox.Show("是否继续对当前轮次重新分组？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                GameService.GetInstance().reSortNewestSwissGroups();
                Turn = GameService.GetInstance().getLastTurn();

                checkAllCommand();

            }

            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);
        }

        #endregion

        #region GameSituationButton

        /// <summary>
        /// 预判哪能否执行CheckPlayersCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canCheckPlayersCommandExecute()
        {
            if (SelectTurn == -1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 查看选手信息（排行版）
        /// </summary>
        /// <param name="win"></param>
        protected override void CheckPlayersCommandExecute()
        {
            GameService.GetInstance().CountAllPlayersPointsByLastTurn();
            GameService.GetInstance().Rank();
            PlayersWindow PlayersWin = PlayersWindow.GetInstance();
            //PlayersWin.Owner = win;
            PlayersWin.Show();


            //PlayersWin.ShowDialog();
        }

        #endregion

        #region SetResultComobox

        /// <summary>
        /// 设置结果
        /// </summary>
        /// <param name="cmbox"></param>
        protected override void SetResultCommandExecute()
        {
            EditedItem = SelectItem;
        }

        /// <summary>
        /// 完成设置结果时
        /// </summary>
        protected override void CellEditEndingCommandExecute()
        {
            Console.WriteLine("正在编辑的是：" + SelectItem);

            if (EditedItem == null)
            {
                return;
            }

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();
            GameService.GetInstance().CountPlayerPointsByLastTurnInGroup(EditedItem);
            GameService.GetInstance().updataRivalTotalPointByGroup(EditedItem);
            GameService.GetInstance().Rank();
            stopwatch.Stop();

            Messenger.Default.Send(new List<Player>(GameService.GetInstance().Game.Players), ViewModelContext.UpdataRank);

            TimeSpan timeSpan = stopwatch.Elapsed;
            Console.WriteLine(timeSpan.TotalSeconds);

            checkAllCommand();

            Console.WriteLine("保存数据");
            GameService.GetInstance().SavePlays2Disk();
            GameService.GetInstance().SaveGameInfo2Disk();

        }

        #endregion

        #region TurnsComobox


        #endregion

        #region OutPutGroupInfoButton

        /// <summary>
        /// 输出当前分组战况信息
        /// </summary>
        protected override void OutPutGroupCommandExecute()
        {
            GameService.GetInstance().OutPutGroupToClipBoard(SelectTurn);
        }

        #endregion


        /// <summary>
        /// 订阅轮次信息改变事件的处理方法
        /// </summary>
        /// <param name="turn"></param>
        private void showTurn(Turn turn)
        {
            Turn = turn;
        }


        protected override string getTurnName(int turnIndex)
        {
            switch (GameService.GetInstance().Game.GameSetting.Turns - turnIndex)
            {
                case 0:
                    return "最终轮";
                default:
                    return "第" + turnIndex + "轮";
            }
        }
    }
}
