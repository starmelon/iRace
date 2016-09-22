using GongSolutions.Wpf.DragDrop;
using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using RaceManagerTool.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;


namespace RaceManagerTool.ViewModels
{
    public class GameInfoWinViewModel : BindableBase
    {

        public int SelectPlayerIndex { get; set; }
       

        private ObservableCollection<Player> players;
        public ObservableCollection<Player> Players
        {
            get { return players; }
            set 
            {
                players = value;
                this.OnPropertyChanged("Players");
            }
        }

        public List<Host> Hosts { get; set; }

        public bool canEditTurns;
        public bool CanEditTurns
        {
            get { return canEditTurns; }
            set
            {
                canEditTurns = value;
                this.OnPropertyChanged("CanEditTurns");
            }
        }

        private GameSetting game;
        public GameSetting Game
        {
            get { return game; }
            set
            {
                game = value;
                this.OnPropertyChanged("Game");
            }
        }

        public DeckDrop Deckdrop { get; set; }

        public DelegateCommand CreateGameCommand { get; set; }
        public DelegateCommand CleanGameSettingCommand { get; set; }
        public DelegateCommand ReadGameCommand { get; set; }

       
        public DelegateCommand LoadPlayersFromDocumentCommand { get; set; }
        public DelegateCommand DeletePlayerCommnd { get; set; }
        public DelegateCommand ReLoadPlayersCommand { get; set; }
        public DelegateCommand OutPutPlayersCommand { get; set; }
        public DelegateCommand SelectGameTypeCommand { get; set; }
        public DelegateCommand SetGameNumCommand { get; set; }

        private bool isCancel;

        public DelegateCommand ActivatedWinCommand { get; set; } 
        public DelegateCommand SaveGameSettingCommand { get; set; }
        public DelegateCommand UnSaveGameSettingCommand { get; set; }
        

        public event EventHandler ClosingRequest;

        protected void OnClosingRequest()
        {
            if (this.ClosingRequest != null)
            {
                this.ClosingRequest(this, EventArgs.Empty);
            }
        }


        public GameInfoWinViewModel()
        {
            Hosts = GameService.GetInstance().Hosts;
            Game = GameService.GetInstance().Game.GameSetting;
            Players = GameService.GetInstance().Game.Players;

            CanEditTurns = true;
            if (Game.GameType.Equals("淘汰赛"))
            {
                CanEditTurns = false;
            }
            

            Deckdrop = new DeckDrop();


            this.CreateGameCommand = new DelegateCommand(new Action(this.CreateGameCommandExecute),new Func<bool>(this.CanCreateGameCommandExecute));
            this.CleanGameSettingCommand = new DelegateCommand(new Action(this.CleanGameSettingCommandExecute),new Func<bool>(this.CanCleanGameSettingCommand));
            this.ReadGameCommand = new DelegateCommand(new Action(this.ReadGameCommandExecute),new Func<bool>(this.CanReadGameCommandExecute));

            this.DeletePlayerCommnd = new DelegateCommand(new Action(this.DeletePlayerCommndExecute),new Func<bool>(this.CanDeletePlayerCommndExecute));

            this.LoadPlayersFromDocumentCommand = new DelegateCommand(new Action(this.LoadPlayerDocumentCommandExecute),new Func<bool>(this.CanLoadPlayerDocumentCommandExecute));
            this.ReLoadPlayersCommand = new DelegateCommand(new Action(this.ReLoadPlayersCommandExecute));
            this.SelectGameTypeCommand = new DelegateCommand(new Action(this.SelectGameTypeCommandExecute));
            this.SetGameNumCommand = new DelegateCommand(new Action(this.SetGameNumCommandExecute));
            this.OutPutPlayersCommand = new DelegateCommand(new Action(this.OutPutPlayersCommandExecute),new Func<bool>(this.CanOutPutPlayersCommandExecute));

            this.ActivatedWinCommand = new DelegateCommand( new Action(this.ActivatedWinCommandExecute));
            this.SaveGameSettingCommand = new DelegateCommand(new Action(this.SaveGameSettingCommandExecute),new Func<bool>(this.CanSaveGameSettingCommandExecute));
            this.UnSaveGameSettingCommand = new DelegateCommand(new Action(this.UnSaveGameSettingCommandExecute),new Func<bool>(this.CanExecuteUnSaveGameSettingCommand));

            Messenger.Default.Register(this, new Action(this.cheAllCommand), ViewModelContext.GameInfoChange);

            ((System.ComponentModel.IEditableObject)Game).BeginEdit();


        }

        


        /// <summary>
        /// 窗体激活时执行的命令
        /// </summary>
        private void ActivatedWinCommandExecute()
        {

            isCancel = true;
            this.UnSaveGameSettingCommand.RaiseCanExecuteChanged();
            ((System.ComponentModel.IEditableObject)Game).BeginEdit();
        }

        

        /// <summary>
        /// 与判断能否执行CreateGameCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanCreateGameCommandExecute()
        {
            if (GameService.GetInstance().Games.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 创建赛事命令
        /// </summary>
        /// <param name="win"></param>
        private void CreateGameCommandExecute()
        {
            ((System.ComponentModel.IEditableObject)Game).EndEdit();

            GameService.GetInstance().CreatGame();

            ((System.ComponentModel.IEditableObject)Game).BeginEdit();

            MessageBox.Show("已创建缓存文件夹");

            cheAllCommand();       
        }

        /// <summary>
        /// 预判能否执行ReadGameCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanReadGameCommandExecute()
        {
            return true;
        }

        /// <summary>
        /// 读取赛事的命令
        /// </summary>
        private void ReadGameCommandExecute()
        {
            
        }

        /// <summary>
        /// 预判能否执行CleanGameSettingCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanCleanGameSettingCommand()
        {
            if (GameService.GetInstance().Games.Count < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 清空删除当前游戏
        /// </summary>
        private void CleanGameSettingCommandExecute()
        {
            if (MessageBox.Show("当前存在赛事，新建将清空本次赛事相关文件，是否继续？", "", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                ((System.ComponentModel.IEditableObject)Game).CancelEdit();

                Messenger.Default.Send(ViewModelContext.CleanGame);
                GameService.GetInstance().CleanGame();
                Game = GameService.GetInstance().Game.GameSetting;
                Players = GameService.GetInstance().Game.Players;

                ((System.ComponentModel.IEditableObject)Game).BeginEdit();

                cheAllCommand();
            }
        }

        /// <summary>
        /// 当设置玩家人数时要处理的命令
        /// </summary>
        private void SetGameNumCommandExecute()
        {
            GameService.GetInstance().UpdataTurns();
            cheAllCommand();
        }

        /// <summary>
        /// 变更比赛模式
        /// </summary>
        private void SelectGameTypeCommandExecute()
        {
            if (Game.GameType.Equals("淘汰赛"))
            {
                CanEditTurns = false;
            }
            else
            {
                CanEditTurns = true;
                
            }
            GameService.GetInstance().UpdataTurns();
        }


        

        /// <summary>
        /// 预判能否执行SaveGameSettingCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanSaveGameSettingCommandExecute()
        {
            if (GameService.GetInstance().Games.Count < 1)
            {
                return false;
            }

            if (GameService.GetInstance().Game.Turns.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保存命令
        /// </summary>
        /// <param name="win"></param>
        private void SaveGameSettingCommandExecute()
        {

            //win.tb_gameName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            //win.cmbox_gameType.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
            //win.cmbox_gameTurns.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();
            //win.cmbox_host.GetBindingExpression(ComboBox.SelectedValueProperty).UpdateSource();

            GameService.GetInstance().checkGameName();

            ((System.ComponentModel.IEditableObject)Game).EndEdit();


            GameService.GetInstance().SaveGameSetting2Disk();
            GameService.GetInstance().SavePlays2Disk();

            Console.WriteLine(GameService.GetInstance().Game.GameSetting.GameType);

            isCancel = false;
            this.OnClosingRequest();

            Messenger.Default.Send(GameService.GetInstance().Game.GameSetting, ViewModelContext.SetGameType);
            //win.Close();


        }

        /// <summary>
        /// 判断能否执行UnSaveGameSettingCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanExecuteUnSaveGameSettingCommand()
        {
            return isCancel;
        }

        /// <summary>
        /// 取消命令
        /// </summary>
        private void UnSaveGameSettingCommandExecute()
        {
            ((System.ComponentModel.IEditableObject)Game).CancelEdit();
        }

        /// <summary>
        /// 预判能否执行OutPutPlayersCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanOutPutPlayersCommandExecute()
        {
            if (GameService.GetInstance().Games.Count < 1)
            {
                return false;
            }

            if (Players.Count < 1)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 输出参赛玩家信息
        /// </summary>
        private void OutPutPlayersCommandExecute()
        {
            GameService.GetInstance().OutPutPlayers();
        }

        /// <summary>
        /// 重载玩家数据命令
        /// </summary>
        private void ReLoadPlayersCommandExecute()
        {
            ReloadPlayers();
        }

        /// <summary>
        /// 重载玩家数据
        /// </summary>
        private void ReloadPlayers()
        {
            string path = null;
            if (GameService.GetInstance().Game.GameSetting.FullNameOld != null)
            {
                path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp\",GameService.GetInstance().Game.GameSetting.FullNameOld,@"Decks");
            }
            else 
            {
                path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp\", GameService.GetInstance().Game.GameSetting.FullName, @"Decks");
            }

            //C#遍历指定文件夹中的所有文件
            DirectoryInfo TheFolder = new DirectoryInfo(path);

            //清空玩家数据
            GameService.GetInstance().Game.Players.Clear();

            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.EndsWith(".ydk"))
                {
                    GameService.GetInstance().AddGamer(NextFile.Name.Replace(".ydk", ""));
                }
                else
                {
                    Console.WriteLine(NextFile.Name);
                }
                
            }

        }

        /// <summary>
        /// 预判能否执行DeletePlayerCommndExecute
        /// </summary>
        /// <returns></returns>
        private bool CanDeletePlayerCommndExecute()
        {

            if (GameService.GetInstance().Game.Turns.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除玩家
        /// </summary>
        private void DeletePlayerCommndExecute()
        {
            if (SelectPlayerIndex == -1)
            {
                return;
            }
            if (GameService.GetInstance().RrmovePlayer(SelectPlayerIndex) == false)
            {
                MessageBox.Show("删除失败");
            }
        }

        /// <summary>
        /// 预判能否执行LoadPlayerDocumentCommandExecute
        /// </summary>
        /// <returns></returns>
        private bool CanLoadPlayerDocumentCommandExecute()
        {
            if (GameService.GetInstance().Games.Count < 1)
            {
                return false;
            }

            if (GameService.GetInstance().Game.Turns.Count > 0)
            {
                return false;
            }

            if (GameService.GetInstance().Game.Players.Count >= GameService.GetInstance().Game.GameSetting.Num)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 通过文件夹读取卡组
        /// </summary>
        /// <param name="win"></param>
        private void LoadPlayerDocumentCommandExecute()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.FullNameOld);

            if (Directory.Exists(path) == false)
            {
                MessageBox.Show("请先创建比赛");
                return;
            }

            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string m_Dir = m_Dialog.SelectedPath.Trim();

            DirectoryInfo TheFolder = new DirectoryInfo(m_Dir);

            int ydkNum = TheFolder.GetFiles().Count(s => s.Extension.Equals(".ydk"));
            int curPlayerNum = GameService.GetInstance().Game.GameSetting.Num - GameService.GetInstance().Game.Players.Count;

            if (ydkNum > curPlayerNum)
            {
                
                return;
            }

            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.EndsWith(".ydk"))
                {
                    string sourcePath = System.IO.Path.Combine(NextFile.DirectoryName, NextFile.Name);
                    string targetPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp\", Game.FullNameOld, @"Decks", NextFile.Name);
                    System.IO.File.Copy(sourcePath, targetPath, true);
                }
            }

            ReloadPlayers();

            cheAllCommand();
        }

        private void cheAllCommand()
        {
            CreateGameCommand.RaiseCanExecuteChanged();
            LoadPlayersFromDocumentCommand.RaiseCanExecuteChanged();
            DeletePlayerCommnd.RaiseCanExecuteChanged();
            CleanGameSettingCommand.RaiseCanExecuteChanged();
            SaveGameSettingCommand.RaiseCanExecuteChanged();
            OutPutPlayersCommand.RaiseCanExecuteChanged();
        }
    }

    

    /// <summary>
    /// 卡组拖放的接口实现类
    /// </summary>
    public class DeckDrop : IDropTarget
    {

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            Console.WriteLine(dropInfo);


            //判断拖进的卡组是否全是ydk，且不超过设定人数
            var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            bool isAllydk = dragFileList.Any(item =>
            {
                var extension = System.IO.Path.GetExtension(item);
                return extension != null && extension.Equals(".ydk");
            });
            if( !isAllydk)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            int curPlayerNum = GameService.GetInstance().Game.GameSetting.Num - GameService.GetInstance().Game.Players.Count;
            if (dragFileList.Count<string>() > curPlayerNum)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (GameService.GetInstance().Game.Players.Count == GameService.GetInstance().Game.GameSetting.Num)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (GameService.GetInstance().Game.Turns.Count > 0)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            if (GameService.GetInstance().Games.Count < 1)
            {
                dropInfo.Effects = DragDropEffects.None;
                return;
            }

            dropInfo.Effects = DragDropEffects.Copy | DragDropEffects.Move;

            //var dragFileList = ((DataObject)dropInfo.Data).GetFileDropList().Cast<string>();
            //dropInfo.Effects = dragFileList.Any(item =>
            //{

            //    var extension = System.IO.Path.GetExtension(item);
            //    return extension != null && extension.Equals(".ydk");
            //}) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", GameService.GetInstance().Game.GameSetting.FullNameOld);

            if (Directory.Exists(path) == false)
            {
                MessageBox.Show("请先创建比赛");
                return;
            }

            //if (GameService.GetInstance().isGameCreate() == false)
            //{
            //    MessageBox.Show("请先创建游戏");
            //    return;
            //}
            GameService.GetInstance().checkGameName();
            GameService.GetInstance().AddGamers((DataObject)dropInfo.Data);
            //GameService.GetInstance().UpdataTurns();
            Messenger.Default.Send(ViewModelContext.GameInfoChange);
        }

    }
}
