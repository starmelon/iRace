using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using RaceManagerTool.Tools;
using RaceManagerTool.View;
using System;
using System.Windows;

namespace RaceManagerTool.ViewModels
{
    class EliminationModeViewModel : GameModeBaseViewModel
    {
       

        public EliminationModeViewModel()
        {
            //订阅轮次更新的消息
            Messenger.Default.Register<Turn>(this, new Action<Turn>(this.showTurn), ViewModelContext.ShowTurns);
 
        }

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
            if (MessageBox.Show("是否删除当前轮次？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                SelectTurn = ListTurns.Count - 2;
                ListTurns.RemoveAt(ListTurns.Count - 1);

                GameService.GetInstance().removeNewestTurn();
                checkAllCommand();
            }      
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
            if (MessageBox.Show("是否重置当前轮次？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                GameService.GetInstance().RestEliminationTurn();
                Turn = GameService.GetInstance().getLastTurn();

                checkAllCommand();
            }    
            
        }


        #endregion

        #region CleanResulsButton

        /// <summary>
        /// 预判能否执行CleanResultsCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canCleanResultsCommand()
        {

            //判断选择的是否是当前轮
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
            if (MessageBox.Show("是否清空本轮分组结果？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                GameService.GetInstance().CleanAllResults();
                checkAllCommand();
            }

            //    if (SelectTurn != ListTurns.Count - 1)
            //{
            //    MessageBox.Show("只允许清空进行中的最后一轮！");
            //    return;
            //}
        }

        #endregion

        #region SetOverTimeButton

        /// <summary>
        /// 预判能否执行SetOverTimeCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canSetOverTimeCommandExecute()
        {
            //未选轮次不允许操作
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
            if (MessageBox.Show("是否对当前剩余分组设置超时？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                GameService.GetInstance().SetEliminationTurnOverTime();
                checkAllCommand();
            }

                

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
        /// 开启下一轮淘汰赛
        /// </summary>
        /// <param name="win"></param>
        protected override void StartNextTurnCommandExecute()
        {
            if (MessageBox.Show("是否已向选手确认当前轮次结果？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (GameService.GetInstance().StartNextEliminationTurn())
                {
                    ListTurns.Add(getTurnName(ListTurns.Count + 1));
                    SelectTurn = ListTurns.Count - 1;

                    Turn = GameService.GetInstance().getLastTurn();

                    checkAllCommand();
                }   
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

            checkAllCommand();
            

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
            if (MessageBox.Show("是否对当前轮次重新分组？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                GameService.GetInstance().reSortNewestEliminationGroups();
                Turn = GameService.GetInstance().getLastTurn();

                checkAllCommand();

            }
        }

        #endregion

        #region GameSituationButton

        /// <summary>
        /// 预判哪能否执行CheckPlayersCommandExecute
        /// </summary>
        /// <returns></returns>
        protected override bool canCheckPlayersCommandExecute()
        {
            return false;
        }

        /// <summary>
        /// 查看选手信息（排行版）
        /// </summary>
        /// <param name="win"></param>
        protected override void CheckPlayersCommandExecute()
        {
            
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

            checkAllCommand();

            GameService.GetInstance().SetResult(EditedItem);


        }

        #endregion

        #region TurnsComobox

        //不重写

        #endregion

        #region OutPutGroupInfoButton

        //不重写

        #endregion


        /// <summary>
        /// 订阅轮次信息改变事件的处理方法
        /// </summary>
        /// <param name="turn"></param>
        private void showTurn(Turn turn)
        {
            Turn = turn;
        }

        /// <summary>
        /// 获取轮次名称
        /// </summary>
        /// <param name="turnIndex"></param>
        /// <returns></returns>
        protected override String getTurnName(int turnIndex)
        {
            
            switch (GameService.GetInstance().Game.GameSetting.Turns - turnIndex)
            {
                case 0:
                    return "决赛";
                case 1:
                    return "半决赛";
                case 2:
                    return "四强";
                case 3:
                    return "八强";
                default:
                    return "第" + turnIndex + "轮";
            }
        }

    }
}
