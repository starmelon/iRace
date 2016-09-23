using IRace.Dao;
using IRace.Tools;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RaceManagerTool.Services
{
    public sealed class GameService:BindableBase
    {
        private static readonly GameService instance = new GameService();

        public static GameService GetInstance()
        {
            return instance;
        }

        private GameService()
        {

        }

        public List<Result> Results { get; set; }
        public List<Host> Hosts { get; set; }

        public ObservableCollection<Game> Games { get; set; }

        private Game game;
        public Game Game
        {
            get
            {
                return game;
            }
            set
            {
                game = value;
                this.OnPropertyChanged("Game");
            }
        }


        #region 比赛设置和信息缓存

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            Results = XmlResultService.GetInstance().GetDefaultResults();
            Hosts = XmlHostService.GetInstance().Host;

            Games = readTempGame();

            if (Games.Count != 0)
            {
                Game = Games[0];
                return;
            }

            setDefaultGame();

        }

        /// <summary>
        /// 设置默认游戏
        /// </summary>
        public void setDefaultGame()
        {
            Game = new Game();
            ((System.ComponentModel.IEditableObject)Game.GameSetting).BeginEdit();
            Game.GameSetting.GameType = "淘汰赛";
            Game.GameSetting.Host = Hosts[0];
            Game.GameSetting.Num = 32;
            UpdataTurns();
            Game.GameSetting.Name = "竞赛联盟AA赛-" + System.DateTime.Now.Year + "年BB场比赛";
            ((System.ComponentModel.IEditableObject)Game.GameSetting).EndEdit();
            
        }

        /// <summary>
        /// 读取缓存文件夹下的赛事
        /// </summary>
        public ObservableCollection<Game> readTempGame()
        {


            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp");


            ObservableCollection<Game> games = new ObservableCollection<Game>();

            DirectoryInfo gameTempFolder = new DirectoryInfo(path);

            foreach (DirectoryInfo nextFolder in gameTempFolder.GetDirectories())
            {
                Game game = new Game();
                string gameSettingPath = System.IO.Path.Combine(nextFolder.FullName, @"info\GameSetting.bin");
                if (File.Exists(gameSettingPath) == false)
                {
                    continue;
                }
                string playersPath = System.IO.Path.Combine(nextFolder.FullName, @"info\Players.bin");
                if (File.Exists(playersPath) == false)
                {
                    continue;
                }
                

                game.GameSetting = LoadGameSettingFromDisk(gameSettingPath);
                game.Players = loadPlaysFromDisk(playersPath);

                string gameInfoPath = System.IO.Path.Combine(nextFolder.FullName, @"info\GameInfo.bin");
                if (File.Exists(gameInfoPath) == true)
                {
                    game.Turns = LoadGameInfoFromDisk(gameInfoPath,game.Players);
                }

                games.Add(game);
            }

            return games;
        }

        /// <summary>
        /// 读取玩家信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ObservableCollection<Player> loadPlaysFromDisk(string path)
        {
            return XMLSerializeUitl.Xml2Object<ObservableCollection<Player>>(path);
        }

        /// <summary>
        /// 读取比赛设置信息
        /// </summary>
        public GameSetting LoadGameSettingFromDisk(String path)
        {

            GameSetting game = null;

            if (File.Exists(path) == true)
            {
                game = XMLSerializeUitl.Xml2Object<GameSetting>(path);
                foreach (var host in Hosts)
                {
                    if (host.QQ.Equals(game.Host.QQ))
                    {
                        game.Host = host;
                    }
                }
            }

            return game;
        }

        /// <summary>
        /// 读取轮次信息
        /// </summary>
        public ObservableCollection<Turn> LoadGameInfoFromDisk(string path,ObservableCollection<Player> players)
        {
            //string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp\info\GameInfo.bin");

            ObservableCollection<Turn> turns = XMLSerializeUitl.Xml2Object<ObservableCollection<Turn>>(path);

            foreach (var turn in turns)
            {
                foreach (var g in turn.Groups)
                {
                    if (g.Resultindex != -1)
                    {
                        g.Result = Results[g.Resultindex];
                    }

                    var player1 =
                        from p1 in players
                        where p1.QQ == g.Q1
                        select p1;
                    if (player1.Count() > 0)
                    {
                        g.Play1 = (Player)player1.Single();
                        g.Play1.Groups.Add(g);
                    }


                    var player2 =
                        from p2 in players
                        where p2.QQ == g.Q2
                        select p2;
                    if (player2.Count() > 0)
                    {
                        g.Play2 = player2.Single() as Player;
                        g.Play2.Groups.Add(g);
                    }

                }
            }

            return turns;
        }

        /// <summary>
        /// 保存比赛设置信息
        /// </summary>
        public void SaveGameSetting2Disk()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp\", Game.GameSetting.FullName, @"info");

            if (Directory.Exists(path) == true)
            {
                string binpath = System.IO.Path.Combine(path, @"GameSetting.bin");
                XMLSerializeUitl.Object2Xml<GameSetting>(Game.GameSetting, binpath);
            }

        }

        /// <summary>
        /// 保存轮次信息
        /// </summary>
        public void SaveGameInfo2Disk()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullName, @"info");

            if (Directory.Exists(path) == true)
            {
                string binpath = System.IO.Path.Combine(path, @"GameInfo.bin");
                XMLSerializeUitl.Object2Xml<ObservableCollection<Turn>>(Game.Turns, binpath);
            }
        }

        /// <summary>
        /// 保存玩家信息
        /// </summary>
        public void SavePlays2Disk()
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullName, @"info");
            string binpath = System.IO.Path.Combine(path, @"Players.bin");
            XMLSerializeUitl.Object2Xml<ObservableCollection<Player>>(Game.Players, binpath);

        }

        /// <summary>
        /// 新建一场比赛
        /// 创建赛事文件夹
        /// </summary>
        public void CreatGame()
        {
            try
            {
                string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullName);
                Directory.CreateDirectory(path);
                string deckspath = System.IO.Path.Combine(path, @"Decks");
                Directory.CreateDirectory(deckspath);
                string infopath = System.IO.Path.Combine(path, @"info");
                Directory.CreateDirectory(infopath);
                Games.Add(Game);
                SaveGameInfo2Disk();
                SaveGameSetting2Disk();
                SavePlays2Disk();
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// 删除当前赛事
        /// </summary>
        public void CleanGame()
        {
            
            string path = Path.Combine(Environment.CurrentDirectory, @"Game\Temp\", Game.GameSetting.FullName);
            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
            }
            Games.Clear();
            Game = null;
            setDefaultGame();
        }

        /// <summary>
        /// 检查变更赛事名称
        /// </summary>
        public void checkGameName()
        {
            if (Game.GameSetting.FullNameOld.Equals(Game.GameSetting.FullName))
            {
                return;
            }

            string pathold = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullNameOld);
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullName);

            try
            {
                Directory.Move(pathold, path);
            }
            catch (Exception)
            {

                
            }
            
        }

        /// <summary>
        /// 更新轮次次数
        /// </summary>
        public void UpdataTurns()
        {
            if (Game.GameSetting.GameType.Equals("淘汰赛"))
            {

                //int playernum = Game.Players.Count;
                int playernum = Game.GameSetting.Num;
                if (playernum > 1)
                {
                    Game.GameSetting.Turns = (int)Math.Ceiling(Math.Log(playernum, 2));
                    Console.WriteLine(playernum + " - " + Math.Ceiling(Math.Log(playernum, 2)));

                }
                else
                {
                    Game.GameSetting.Turns = 0;
                }
            }
            else
            {
                Game.GameSetting.Turns = 4;
            }
        }

        /// <summary>
        /// 添加多个参赛玩家
        /// </summary>
        /// <param name="decks"></param>
        public void AddGamers(DataObject decks)
        {
            var dragFileList = decks.GetFileDropList().Cast<string>();

            foreach (var item in dragFileList)
            {
                var extension = System.IO.Path.GetExtension(item);
                if (extension != null && extension.Equals(".ydk"))
                {
                    string sourcePath = System.IO.Path.GetFullPath(item);
                    string targetPath = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullNameOld, @"Decks\", System.IO.Path.GetFileName(item));
                    if (File.Exists(targetPath) == false)
                    {
                        System.IO.File.Copy(sourcePath, targetPath, true);
                        AddGamer(System.IO.Path.GetFileNameWithoutExtension(item));
                    }

                }
            }
            SavePlays2Disk();
        }

        /// <summary>
        /// 添加参赛玩家
        /// </summary>
        /// <param name="filename"></param>
        public void AddGamer(string filename)
        {
            string[] infos = filename.Split('+');
            if (infos.Length != 2)
            {
                infos = filename.Split('＋');
            }

            if (infos.Length == 2)
            {
                Player gamer = new Player();
                gamer.QQ = infos[0];
                for (int i = 1; i < infos.Length; i++)
                {
                    gamer.Name += infos[1];
                }
                //gamer.Name = ;
                Game.Players.Add(gamer);
            }
            else
            {

                MessageBox.Show("检测到无法识别的文件名" + filename);
            }

        }

        /// <summary>
        /// 删除参赛玩家
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RrmovePlayer(int index)
        {
            Player p = Game.Players[index];

            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullNameOld, @"Decks\" + p.QQ + "+" + p.Name + ".ydk");
            if (File.Exists(path) == false)
            {
                path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullNameOld, @"Decks\", p.QQ + "＋" + p.Name + ".ydk");
            }
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Game.Players.RemoveAt(index);
                    GameService.GetInstance().UpdataTurns();
                    SavePlays2Disk();
                }
                catch (Exception)
                {
                    return false;
                }

            }

            return true;

        }

        #endregion

        #region 比赛操作相关

        /// <summary>
        /// 开启比赛
        /// </summary>
        /// <returns></returns>
        public bool StartNewGame()
        {
            createNewEliminationTurnByPlayers(GameService.GetInstance().Game.Players);
            if (Game.Turns.Count == 0)
            {
                return false;
            }

            SaveGameInfo2Disk();

            TestSetResult();

            return true;
        }

        #region 瑞士轮操作

        /// <summary>
        /// 根据玩家名单创建新的瑞士轮轮次
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        private void createNewEliminationTurnByPlayers(ObservableCollection<Player> players)
        {
            Turn newTurn = new Turn();
            newTurn.TurnIndex = Game.Turns.Count + 1;

            //求当前轮次的分组数
            int totalPlayerNum = Game.GameSetting.Num;
            int groupnum = totalPlayerNum / (int)(Math.Pow(2, newTurn.TurnIndex));
            setTurnGroupByPlayerAndGroupNum(players, newTurn, groupnum);
        }

        

        /// <summary>
        /// 重置当前瑞士轮轮次
        /// </summary>
        public void RestSwissTurn()
        {
            CleanAllResults();
            GameService.GetInstance().CountAllPlayersPointsByLastTurn();
        }

        /// <summary>
        /// 开启下一轮瑞士轮
        /// </summary>
        /// <returns></returns>
        public bool StartNextSwissTurn()
        {
            foreach (Group group in Game.Turns[Game.Turns.Count - 1].Groups)
            {
                if (group.Result == null || group.Result.Equals(""))
                {
                    return false;
                }
            }

            int playernum = Game.Players.Count;

            Turn turn = new Turn();
            Group firstgroup = new Group();
            firstgroup.TurnIndex = Game.Turns.Count;
            firstgroup.Num = 1;
            turn.Groups.Add(firstgroup);
            foreach (var player in Game.Players)
            {
                if (player.Status == false)
                {
                    Group group = turn.Groups[turn.Groups.Count - 1];
                    if (group.Play1 == null)
                    {
                        group.Play1 = player;
                        player.Groups.Add(group);
                    }
                    else if (group.Play2 == null)
                    {
                        group.Play2 = player;
                        player.Groups.Add(group);
                    }
                    else
                    {
                        Group nextgroup = new Group();
                        nextgroup.TurnIndex = Game.Turns.Count;
                        nextgroup.Num = group.Num + 1;
                        turn.Groups.Add(nextgroup);

                        nextgroup.Play1 = player;
                        player.Groups.Add(nextgroup);
                    }

                }
            }
            Game.Turns.Add(turn);

            return true;
        }

        /// <summary>
        /// 对瑞士轮最新轮次进行分组
        /// </summary>
        public void reSortNewestSwissGroups()
        {
            ObservableCollection<Player> players = getNewestPlayers();

            removeNewestTurn();
            createNewSwissTurnByPlayer(players);

            SaveGameInfo2Disk();
        }

        #endregion

        #region 瑞士轮积分操作

        /// <summary>
        /// 计算当前轮次所有选手总分
        /// </summary>
        public void CountAllPlayersPointsByLastTurn()
        {
            int turnnum = Game.Turns.Count;
            CountAllPlayersPointsByTurn(turnnum);
            foreach (var player in Game.Players)
            {
                calcRivalTotalPoints(player);
            }

        }

        /// <summary>
        /// 根据轮次，计算所有选手总分
        /// </summary>
        /// <param name="turnnum">轮次</param>
        public void CountAllPlayersPointsByTurn(int turnnum)
        {
            foreach (Player player in Game.Players)
            {
                CountPlayerPointByTurn(player, turnnum);
            }
        }

        /// <summary>
        /// 根据分组，计算分组中选手到最新轮次的总分
        /// </summary>
        /// <param name="group">分组</param>
        public void CountPlayerPointsByLastTurnInGroup(Group group)
        {
            int turnnum = GameService.GetInstance().Game.Turns.Count;
            if (group.Play1 != null)
            {
                CountPlayerPointByTurn(group.Play1, turnnum);
            }
            if (group.Play2 != null)
            {
                CountPlayerPointByTurn(group.Play2, turnnum);
            }

        }

        /// <summary>
        /// 根据轮次，计算一个选手的总积分，累进分，中间分，输赢差
        /// </summary>
        /// <param name="player"></param>
        /// <param name="turnnum"></param>
        private void CountPlayerPointByTurn(Player player, int turnnum)
        {
            int total = 0;

            //小局结果
            int win = 0;
            int tie = 0;
            int lose = 0;

            //大局结果
            int winturn = 0;
            int tieturn = 0;
            int loseturn = 0;

            //累进分
            int progressive = 0;

            //中间分
            int highest = 0;
            int lowest = 0;


            foreach (var group in player.Groups)
            {
                if (group.Result == null)
                {
                    continue;
                }

                if (group.TurnIndex > turnnum)
                {
                    break;
                }

                if (group.Play1 != null && player.QQ == group.Play1.QQ)
                {

                    win += group.Result.Win;
                    tie += group.Result.Tie;
                    lose += group.Result.Lose;

                    if (group.Result.Win > group.Result.Lose)
                    {
                        winturn += 1;
                    }
                    else if (group.Result.Win < group.Result.Lose)
                    {
                        loseturn += 1;
                    }
                    else if (group.Result.Win == group.Result.Lose && ((group.Result.Win + group.Result.Tie + group.Result.Lose) != 0))
                    {
                        tieturn += 1;
                    }

                    total += group.Result.PointLeft;
                    if (group.Result.PointLeft > highest)
                    {
                        highest = group.Result.PointLeft;
                    }
                    else if (group.Result.PointLeft < lowest)
                    {
                        lowest = group.Result.PointLeft;
                    }

                }
                else if (group.Play2 != null && player.QQ == group.Play2.QQ)
                {
                    win += group.Result.Lose;
                    tie += group.Result.Tie;
                    lose += group.Result.Win;

                    if (group.Result.Win > group.Result.Lose)
                    {
                        loseturn += 1;
                    }
                    else if (group.Result.Win < group.Result.Lose)
                    {
                        winturn += 1;
                    }
                    else if (group.Result.Win == group.Result.Lose && ((group.Result.Win + group.Result.Tie + group.Result.Lose) != 0))
                    {
                        tieturn += 1;
                    }

                    total += group.Result.PointRight;
                    if (group.Result.PointRight > highest)
                    {
                        highest = group.Result.PointRight;
                    }
                    else if (group.Result.PointRight < lowest)
                    {
                        lowest = group.Result.PointRight;
                    }
                }

                progressive = progressive + total;

            }

            player.Win = win;
            player.Tie = tie;
            player.Lose = lose;
            player.WinTurn = winturn;
            player.TieTurn = tieturn;
            player.LoseTurn = loseturn;
            player.TotalPoints = total;
            player.ProgressivePoints = progressive;
            player.MiddlePoints = total - highest - lowest;
            player.WinLoseDifference = win - lose;


        }

        /// <summary>
        /// 更新小组中选手各自所有轮次中的对手的对手分
        /// </summary>
        /// <param name="group"></param>
        public void updataRivalTotalPointByGroup(Group group)
        {
            updataRivalTotalPointByPlayer(group.Play1);
            updataRivalTotalPointByPlayer(group.Play2);
        }

        /// <summary>
        /// 更新小组关联成员的对手分
        /// </summary>
        /// <param name="group"></param>
        public void updataRivalTotalPointByPlayer(Player player)
        {
            if (player == null)
            {
                return;
            }
            foreach (var g in player.Groups)
            {

                if (g.Play1 != player)
                {
                    calcRivalTotalPoints(g.Play1);

                }
                else
                {
                    if (g.Play2 == null)
                    {
                        return;
                    }
                    calcRivalTotalPoints(g.Play2);

                }

            }
        }

        public void calcRivalTotalPoints(Player player)
        {
            int rivaltotal = 0;
            foreach (var group in player.Groups)
            {
                if (player != group.Play1 && group.Play1 != null)
                {
                    rivaltotal += group.Play1.TotalPoints;
                }
                else if (player != group.Play2 && group.Play2 != null)
                {
                    rivaltotal += group.Play2.TotalPoints;
                }
                //rivaltotal += (player == group.Play1 ? (group.Play2 == null ? 0 : group.Play2.TotalPoints) : group.Play1.TotalPoints);
            }
            player.RivalTotalPoints = rivaltotal;
            Console.WriteLine(player.QQ + " -  R =" + player.RivalTotalPoints);
        }

        /// <summary>
        /// 进行排位
        /// </summary>
        public void Rank()
        {
            ObservableCollection<Player> temp;
            temp = new ObservableCollection<Player>(
                Game.Players.OrderByDescending(p => p.TotalPoints)
                .ThenByDescending(p => p.ProgressivePoints)
                .ThenByDescending(p => p.MiddlePoints)
                .ThenByDescending(p => p.RivalTotalPoints)
                .ThenByDescending(p => p.WinLoseDifference));
            Game.Players.Clear();
            foreach (Player p in temp)
            {

                Game.Players.Add(p);
                p.Ranking = Game.Players.Count;
            }


            //Gamers.OrderByDescending(a => a.Points);

        }

        #endregion

        #region 淘汰赛操作

        /// <summary>
        /// 重置当前淘汰赛轮次
        /// </summary>
        public void RestEliminationTurn()
        {
            removeNewestTurn();
            StartNextEliminationTurn();
            SaveGameInfo2Disk();
        }
 
        /// <summary>
        /// 开启下一轮淘汰赛
        /// </summary>
        /// <returns></returns>
        public bool StartNextEliminationTurn()
        {
            

            Turn newTurn = new Turn();
            newTurn.TurnIndex = Game.Turns.Count + 1;

            #region


            Turn lastTurn = getLastTurn();

            int totalPlayerNum = Game.GameSetting.Num;
            int curTurnIndex = Game.Turns.Count;
            int groupnum = totalPlayerNum / (int)(Math.Pow(2, newTurn.TurnIndex));
            Console.WriteLine("轮次"+ newTurn.TurnIndex + "+ "+ groupnum + "组");
            

            for (int i = 0; i < groupnum; i++)
            {
                Group group = new Group();
                group.TurnIndex = newTurn.TurnIndex;
                group.Num = i + 1;
                newTurn.Groups.Add(group);

                if (i * 2 < lastTurn.Groups.Count)
                {
                    if ((lastTurn.Groups[i * 2].Play1 == null && lastTurn.Groups[i * 2].Play2 != null) ||
                            (lastTurn.Groups[i * 2].Play1 != null && lastTurn.Groups[i * 2].Play2 == null))//如果该组一人轮空
                    {
                        Player p = (lastTurn.Groups[i * 2].Play1 == null ? lastTurn.Groups[i * 2].Play2 : lastTurn.Groups[i * 2].Play1);
                        group.Play1 = p;
                        p.Groups.Add(group);
                    }
                    else if (lastTurn.Groups[i * 2].Play1 != null && lastTurn.Groups[i * 2].Play2 != null)//不轮空
                    {
                        if (lastTurn.Groups[i * 2].Result.Win > lastTurn.Groups[i * 2].Result.Lose)
                        {
                            group.Play1 = lastTurn.Groups[i * 2].Play1;
                            group.Play1.Groups.Add(group);
                            lastTurn.Groups[i * 2].Play2.FinalTurnIndex = lastTurn.TurnIndex;

                        }
                        else if (lastTurn.Groups[i * 2].Result.Win < lastTurn.Groups[i * 2].Result.Lose)
                        {
                            group.Play1 = lastTurn.Groups[i * 2].Play2;
                            group.Play1.Groups.Add(group);
                            lastTurn.Groups[i * 2].Play1.FinalTurnIndex = lastTurn.TurnIndex;
                        }
                        else if(lastTurn.Groups[i * 2].Result.Win == 0 && lastTurn.Groups[i * 2].Result.Lose == 0)
                        {
                            lastTurn.Groups[i * 2].Play1.FinalTurnIndex = lastTurn.TurnIndex;
                            lastTurn.Groups[i * 2].Play2.FinalTurnIndex = lastTurn.TurnIndex;
                        }
                    }

                    //if (lastTurn.Groups[i * 2].Result.Win != 0 && lastTurn.Groups[i * 2].Result.Lose != 0)//不是双败的情况
                    //{
                        
                    //}
                }

  
                if (i * 2 + 1  < lastTurn.Groups.Count)
                {
                    if ((lastTurn.Groups[i * 2 + 1].Play1 == null && lastTurn.Groups[i * 2 + 1].Play2 != null) ||
                    (lastTurn.Groups[i * 2 + 1].Play1 != null && lastTurn.Groups[i * 2 + 1].Play2 == null))//如果该组一人轮空
                    {
                        Player p = (lastTurn.Groups[i * 2 + 1].Play1 == null ? lastTurn.Groups[i * 2 + 1].Play2 : lastTurn.Groups[i * 2 + 1].Play1);
                        group.Play2 = p;
                        p.Groups.Add(group);
                    }
                    else if (lastTurn.Groups[i * 2 + 1].Play1 != null && lastTurn.Groups[i * 2 + 1].Play2 != null)//不轮空
                    {
                        if (lastTurn.Groups[i * 2 + 1].Result.Win > lastTurn.Groups[i * 2 + 1].Result.Lose)
                        {
                            group.Play2 = lastTurn.Groups[i * 2 + 1].Play1;
                            group.Play2.Groups.Add(group);
                            lastTurn.Groups[i * 2 + 1].Play2.FinalTurnIndex = lastTurn.TurnIndex;
                        }
                        else if (lastTurn.Groups[i * 2 + 1].Result.Win < lastTurn.Groups[i * 2 + 1].Result.Lose)
                        {
                            group.Play2 = lastTurn.Groups[i * 2 + 1].Play2;
                            group.Play2.Groups.Add(group);
                            lastTurn.Groups[i * 2 + 1].Play1.FinalTurnIndex = lastTurn.TurnIndex;
                        }
                        else if (lastTurn.Groups[i * 2 + 1].Result.Win == 0 && lastTurn.Groups[i * 2 + 1].Result.Lose == 0)
                        {
                            lastTurn.Groups[i * 2 + 1].Play1.FinalTurnIndex = lastTurn.TurnIndex;
                            lastTurn.Groups[i * 2 + 1].Play2.FinalTurnIndex = lastTurn.TurnIndex;
                        }
                    }
                }

            }

            if (groupnum == 1)//分组仅有1，则是决赛
            {
                Group group;
                if (lastTurn.Groups.Count > 0)//上一轮分组大于0则创建季军组
                {
                    group = new Group();
                    group.TurnIndex = newTurn.TurnIndex;
                    group.Num = newTurn.Groups.Count + 1;
                    newTurn.Groups.Add(group);

                    //季军组的第一位选手
                    if (lastTurn.Groups[0].Play1 != null && lastTurn.Groups[0].Play2 != null)
                    {

                        if (lastTurn.Groups[0].Result.Win > lastTurn.Groups[0].Result.Lose)
                        {
                            group.Play1 = lastTurn.Groups[0].Play2;
                            group.Play1.Groups.Add(group);
                        }
                        else if (lastTurn.Groups[0].Result.Win < lastTurn.Groups[0].Result.Lose)
                        {
                            group.Play1 = lastTurn.Groups[0].Play1;
                            group.Play1.Groups.Add(group);
                        }
                    }

                    //若上一轮有第二组，则有季军组的第二位选手
                    if (lastTurn.Groups.Count == 2)
                    {
                        if (lastTurn.Groups[1].Play1 != null && lastTurn.Groups[1].Play2 != null)
                        {
                            if (lastTurn.Groups[1].Result.Win > lastTurn.Groups[1].Result.Lose)
                            {
                                group.Play2 = lastTurn.Groups[1].Play2;
                                group.Play2.Groups.Add(group);
                            }
                            else if (lastTurn.Groups[1].Result.Win < lastTurn.Groups[1].Result.Lose)
                            {
                                group.Play2 = lastTurn.Groups[1].Play1;
                                group.Play2.Groups.Add(group);
                            }
                        }
                    }
                }


            }

            Game.Turns.Add(newTurn);

            SaveGameInfo2Disk();
            SavePlays2Disk();

            TestSetResult();

            #endregion


            return true;
        }

        /// <summary>
        /// 对淘汰赛最新轮次进行分组
        /// </summary>
        public void reSortNewestEliminationGroups()
        {
            
            ObservableCollection<Player> players = getNewestPlayers();
           
            removeNewestTurn();
            createNewEliminationTurnByPlayers(players);

            SaveGameInfo2Disk();
            
        }

        /// <summary>
        /// 根据玩家创建新的淘汰赛轮次
        /// </summary>
        /// <param name="players"></param>
        private void createNewSwissTurnByPlayer(ObservableCollection<Player> players)
        {
            Turn newTurn = new Turn();
            newTurn.TurnIndex = Game.Turns.Count + 1;

            //求当前轮次的分组数
            int totalPlayerNum = Game.GameSetting.Num;
            int groupnum = totalPlayerNum / (int)(Math.Pow(2, 1));
            setTurnGroupByPlayerAndGroupNum(players, newTurn, groupnum);
        }

        #endregion

        /// <summary>
        /// 根据玩家和分组数设置轮次分组
        /// </summary>
        /// <param name="players"></param>
        /// <param name="newTurn"></param>
        /// <param name="groupnum"></param>
        private void setTurnGroupByPlayerAndGroupNum(ObservableCollection<Player> players, Turn newTurn, int groupnum)
        {
            //新轮次的玩家总数
            int playersNum = players.Count();

            if (playersNum < 1)
            {
                return;
            }

            //生成与玩家数量等同的一组随机不重复数组
            int[] playerindexs = ReshuffleByNum(playersNum);

            for (int i = 0; i < groupnum; i++)
            {
                Group group = new Group();
                group.TurnIndex = newTurn.TurnIndex;
                group.Num = i + 1;
                newTurn.Groups.Add(group);

                if (i * 2 < playersNum)
                {
                    //以随机数组中的数字作为index来取玩家List中的玩家
                    group.Play1 = players[playerindexs[i * 2]];
                    group.Play1.Groups.Add(group);
                    group.Play1.FinalTurnIndex = -1;
                }

                if (i * 2 + 1 < playersNum)
                {
                    group.Play2 = players[playerindexs[i * 2 + 1]];
                    group.Play2.Groups.Add(group);
                    group.Play2.FinalTurnIndex = -1;
                }
            }

            Game.Turns.Add(newTurn);
        }

        /// <summary>
        /// 删除当前最新轮次
        /// </summary>
        public void removeNewestTurn()
        {
            foreach (Group group in Game.Turns[Game.Turns.Count - 1].Groups)
            {
                //玩家释放当前轮所在分组信息
                if (group.Play1 != null)
                {
                    group.Play1.Groups.RemoveAt(group.Play1.Groups.Count - 1);
                }
                if (group.Play2 != null)
                {
                    group.Play2.Groups.RemoveAt(group.Play2.Groups.Count - 1);
                }

                //分组释放玩家信息
                group.Dispose();
            }

            //最后轮次释放所有分组信息
            Game.Turns[Game.Turns.Count - 1].Groups.Clear();
            Game.Turns[Game.Turns.Count - 1] = null;
            Game.Turns.RemoveAt(Game.Turns.Count - 1);

            CountAllPlayersPointsByLastTurn();

            GameService.GetInstance().SaveGameInfo2Disk();
        }

        /// <summary>
        /// 获取最新轮次
        /// </summary>
        /// <returns></returns>
        public Turn getLastTurn()
        {
            return Game.Turns.Count != 0 ? Game.Turns[Game.Turns.Count - 1] : null;
        }

        /// <summary>
        /// 获取所有未晋级的玩家
        /// </summary>
        /// <returns></returns>
        public List<Player> getLosePlayers()
        {
            //从所有玩家中
            List<Player> losers = new List<Player>(
                from p in Game.Players

                where p.FinalTurnIndex != -1
                select p);

            losers = losers.OrderByDescending(s => s.FinalTurnIndex).ToList(); ;

            return losers;
        }

        /// <summary>
        /// 获取当前轮的玩家
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<Player> getNewestPlayers()
        {
            ObservableCollection<Player> livePlayers = new ObservableCollection<Player>();

            Turn curturn = getLastTurn();
            if (curturn == null)
            {
                return Game.Players;
            }
            else
            {
                foreach (var group in getLastTurn().Groups)
                {
                    if (group.Play1 != null)
                    {
                        livePlayers.Add(group.Play1);
                    }

                    if (group.Play2 != null)
                    {
                        livePlayers.Add(group.Play2);
                    }
                }
            }



            return livePlayers;
        }

        /// <summary>
        /// 获取当前轮玩家数量
        /// </summary>
        /// <returns></returns>
        private int getWinPlayersNum()
        {
            return getNewestPlayers().Count;
        }

        /// <summary>
        /// 是否正查看当前所有轮次的最后一轮
        /// </summary>
        /// <returns></returns>
        public bool isFinalTurn()
        {
            if (Game.GameSetting.Turns == Game.Turns.Count)
            {
                return true;
            }

            return false;
        }

        public Game getGameByIndex(int index)
        {
            if (index >= Games.Count)
            {
                return null;
            }
            return Games[index];
        }

        /// <summary>
        /// 获取随机数组
        /// </summary>
        /// <param name="playerindex"></param>
        public int[] ReshuffleByNum(int num)
        {
            int[] nums = new int[num];
            for (int i = 0; i < num; i++)
            {
                nums[i] = i;
            }

            Random ram = new Random();
            int currentIndex;
            int tempValue;
            for (int i = 0; i < num; i++)
            {
                currentIndex = ram.Next(0, num - i);
                tempValue = nums[currentIndex];
                nums[currentIndex] = nums[num - 1 - i];
                nums[num - 1 - i] = tempValue;
            }

            return nums;
        }

        #endregion

        #region 对玩家的处理

        public void SetResult(Group group)
        {
            SetLastTurnIndex(group);
            SavePlays2Disk();
            SaveGameInfo2Disk();
        }
        

        public void SetLastTurnIndex(Group group)
        {
            if (group == null)
            {
                return;
            }

            if (group.Play1 == null)
            {
                if (group.Play2 != null)
                {
                    group.Play2.FinalTurnIndex = -1;
                }

            }
            else if (group.Play2 == null)
            {
                group.Play1.FinalTurnIndex = -1;
            }
            else
            {
                if (group.Result != null)
                {
                    if (group.Result.Win > group.Result.Lose)
                    {
                        group.Play1.FinalTurnIndex = -1;
                        group.Play2.FinalTurnIndex = Game.Turns.Count;
                    }
                    else if (group.Result.Win < group.Result.Lose)
                    {
                        group.Play1.FinalTurnIndex = Game.Turns.Count;
                        group.Play2.FinalTurnIndex = -1;
                    }
                }
            }
        }

        #endregion

        #region 信息输出相关

        public void OutPutPlayers()
        {
            StringBuilder strBuilder = new StringBuilder();
            int playersnum = Game.Players.Count;
            for (int i = 0; i < playersnum; i++)
            {
                strBuilder.AppendLine(
                    "[" + (i + 1).ToString().PadLeft(2, '0') + "]" +
                    "[" + Game.Players[i].QQ + "]" +
                    "[" + Game.Players[i].Name + "]");
                Console.WriteLine("[" + (i + 1) + "]" +
                    "[" + Game.Players[i].QQ + "]" +
                    "[" + Game.Players[i].Name + "]");
            }
            //Console.WriteLine(strBuilder.ToString());
            Clipboard.Clear();
            Clipboard.SetText(strBuilder.ToString());
        }

        /// <summary>
        /// 复制分组战况表到剪贴板
        /// </summary>
        /// <param name="turnindex"></param>
        public void OutPutGroupToClipBoard(int turnindex)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("<--- " + Game.GameSetting.FullName + " --->");
            strBuilder.AppendLine("---------------------------------------------");
            strBuilder.AppendLine("第[ " + (turnindex + 1) + " ]轮对决战况表");
            strBuilder.AppendLine("---------------------------------------------");
            foreach (Group group in Game.Turns[turnindex].Groups)
            {
                strBuilder.AppendLine(
                    "[" + group.Num.ToString().PadLeft(2, '0') + "]组");
                strBuilder.AppendLine(
                    "   [选手A][" + (group.Play1 == null ? "" : group.Play1.QQ) + "][" + (group.Play1 == null ? "" : group.Play1.Name) + "]" +
                    (group.Result != null && (group.Result.Win > group.Result.Lose) ? "[√]" : ""));
                strBuilder.AppendLine(
                    "   [比 分][" + (group.Result == null || group.Result.Equals("") ? "VS" : group.Result.Define) + "]");

                //(String.IsNullOrWhiteSpace(group.Remark) == false ?
                //    "[判罚：" + group.Remark + "]" : "")


                strBuilder.AppendLine(
                    "   [选手B][" + (group.Play2 == null ? "" : group.Play2.QQ) + "][" + (group.Play2 == null ? "" : group.Play2.Name) + "]" +
                    (group.Result != null && (group.Result.Lose > group.Result.Win) ? "[√]" : ""));
            }
            strBuilder.AppendLine("---------------------------------------------");
            Turn turn = Game.Turns[turnindex];
            if (turn.ReLifeList.Count > 0)
            {
                strBuilder.AppendLine("以下为复活名单：");

                int num = 1;
                foreach (var p in turn.ReLifeList)
                {
                    foreach (var pcur in Game.Players)
                    {
                        if (p.Key.Equals(pcur.QQ))
                        {
                            strBuilder.AppendLine("[" +  num.ToString().PadLeft(2, '0') + "]"+"[" + pcur.QQ + "]" + "[" + pcur.Name + "]" + "["+ (p.Value ? "√": "×") + "]");
                        }
                    }
                    num++;
                }
                strBuilder.AppendLine("---------------------------------------------");
            }
            strBuilder.AppendLine("请选手确认分组或战况(超时不确认此表将生效)");
            strBuilder.AppendLine("---------------------------------------------");

            Clipboard.Clear();
            Clipboard.SetText(strBuilder.ToString());
        }

        /// <summary>
        /// 复制积分表到剪贴板
        /// </summary>
        public void OutPutRank()
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (Player player in Game.Players)
            {
                strBuilder.AppendLine(
                    "[" + player.Ranking + "]" +
                    "[" + player.QQ + "]" +
                    "[" + player.Name + "]" +
                    "[" + player.WinTurn + "-" + player.TieTurn + "-" + player.LoseTurn + "]" +
                    "[" + player.TotalPoints + "]");
            }
            Clipboard.Clear();
            Clipboard.SetText(strBuilder.ToString());
        }

        /// <summary>
        /// 复制
        /// </summary>
        public void OutPutSelectReLivePlayers(List<Player> players)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine("[落败轮次 - QQ - 昵称]");
            strBuilder.AppendLine("---------------------------------------------");
            foreach (Player player in players)
            {
                strBuilder.AppendLine(
                    "[" + player.FinalTurnIndex + "]" +
                    "[" + player.QQ + "]" +
                    "[" + player.Name + "]");
            }
            strBuilder.AppendLine("---------------------------------------------");
            strBuilder.AppendLine("请参加复活的选手确认(超时不确认此表将生效)");
            strBuilder.AppendLine("---------------------------------------------");
            Clipboard.Clear();
            Clipboard.SetText(strBuilder.ToString());
        }

        #endregion

        #region 比分结果相关

        /// <summary>
        /// 是否已设置全部结果
        /// </summary>
        /// <returns></returns>
        public bool isAllResultsSet()
        {
            if (Game.Turns.Count == 0)
            {
                return false;
            }

            foreach (Group group in Game.Turns[Game.Turns.Count - 1].Groups)
            {
                if (group.Result == null || group.Result.Equals(""))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断当前最新分组是否存在已设置结果的分组
        /// </summary>
        /// <returns></returns>
        public bool isExistResultSet()
        {
            if (Game.Turns.Count == 0)
            {
                return false;
            }

            foreach (Group group in Game.Turns[Game.Turns.Count - 1].Groups)
            {
                if (group.Result != null)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置当前瑞士轮未设置结果集的分组结果为最后一个结果选项
        /// </summary>
        public void SetSwissTurnOverTime()
        {
            SetTurnOverTime();
            GameService.GetInstance().CountAllPlayersPointsByLastTurn();
            GameService.GetInstance().Rank();
        }

        /// <summary>
        /// 设置当前淘汰轮未设置结果集的分组结果为最后一个结果选项
        /// </summary>
        public void SetEliminationTurnOverTime()
        {
            SetTurnOverTime();
        }

        /// <summary>
        /// 设置未设置结果集的分组结果为最后一个结果选项
        /// </summary>
        private void SetTurnOverTime()
        {
            foreach (Group group in Game.Turns[Game.Turns.Count - 1].Groups)
            {
                if (group.Result == null || group.Result.Equals(""))
                {
                    group.Result = Results[Results.Count - 1];
                    //group.Result = "Double Lose";
                }
            }

            GameService.GetInstance().SaveGameInfo2Disk();
 
        }

        /// <summary>
        /// 清空当前所有轮次的最后一轮的所有结果
        /// </summary>
        public void CleanAllResults()
        {
            Turn turn = getLastTurn();
            foreach (var group in turn.Groups)
            {
                group.Result = null;
            }

            SaveGameInfo2Disk();
        }

        #endregion

        #region 复活相关

        /// <summary>
        /// 判断是否能复活
        /// </summary>
        /// <returns></returns>
        public bool canRelive()
        {
            Turn turn = getLastTurn();

            

            if (turn == null)
            {
                return false;
            }

            if (turn.TurnIndex == 1)
            {
                return false;
            }

            bool existEmtyPlayer = false ;
            foreach (var group in turn.Groups)
            {
                if (group.Result != null)
                {
                    return false;
                }
                if (group.Play1 == null || group.Play2 == null)
                {
                    existEmtyPlayer = true;
                }
            }
            if (getRelvieNumMax() > 0)
            {
                return true;
            }
            return existEmtyPlayer;
        }

        /// <summary>
        /// 获取可复活的人数
        /// </summary>
        /// <returns></returns>
        public int getRelvieNumMax()
        {
            int curPlayersNum = getWinPlayersNum();
            int totalPlayerNum = Game.GameSetting.Num;
            int curTurnIndex = Game.Turns.Count;

            int reLiveNum = (totalPlayerNum / (int)(Math.Pow(2, curTurnIndex))) * 2 - curPlayersNum;

            return reLiveNum;
        }

        /// <summary>
        /// 根据复活名单复活选手
        /// </summary>
        /// <param name="joinRelivePlayers"></param>
        public void relive(List<Player> joinRelivePlayers)
        {
            if (joinRelivePlayers == null || joinRelivePlayers.Count == 0)
            {
                return;
            }

            int reliveNum = getRelvieNumMax();

            long tick = DateTime.Now.Ticks;
            Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            

            ObservableCollection<Player> players = getNewestPlayers();

            SerializableDictionary<string, bool> reliveList = getLastTurn().ReLifeList;

            if (joinRelivePlayers.Count() <= reliveNum)
            {
                players.AddRange(joinRelivePlayers);
                foreach (var jplayer in joinRelivePlayers)
                {
                    reliveList.Add(jplayer.QQ, true);
                }
            }
            else
            {
                for (int i = 0; i < reliveNum; i++)
                {
                    int join = joinRelivePlayers.Count();
                    int reliveindex = ran.Next(join);
                    Player player = joinRelivePlayers[reliveindex];
                    joinRelivePlayers.RemoveAt(reliveindex);
                    players.Add(player);
                    reliveList.Add(player.QQ, true);
                }
                foreach (var jplayer in joinRelivePlayers)
                {
                    reliveList.Add(jplayer.QQ, false);
                }
            }

            removeNewestTurn();
            createNewEliminationTurnByPlayers(players);

            getLastTurn().ReLifeList = reliveList;

            SaveGameInfo2Disk();
        }

        #endregion

        /// <summary>
        /// 打包
        /// </summary>
        public void EndToZipGame()
        {
            string packPath = Path.Combine(Environment.CurrentDirectory, @"Game\Temp", Game.GameSetting.FullName);
            string savePath = Path.Combine(Environment.CurrentDirectory, @"Game\Finish", Game.GameSetting.FullName);
            ZipDao.ZipGame(Game.GameSetting.FullName, packPath, savePath);
        }

        public void TestSetResult()
        {


            //Turn turn = getLastTurn();

            //int iSeed = 10;
            //Random ro = new Random(iSeed);
            //long tick = DateTime.Now.Ticks;
            //Random ran = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));

            //foreach (var g in turn.Groups)
            //{
            //    if (g.Play1 == null && g.Play2 != null)
            //    {
            //        g.Result = Results[5];
            //    }
            //    else if (g.Play1 != null && g.Play2 == null)
            //    {
            //        g.Result = Results[4];
            //    }
            //    else if (g.Play1 == null && g.Play2 == null)
            //    {
            //        g.Result = Results[6];
            //    }
            //    else
            //    {
            //        g.Result = Results[ran.Next(7)];
            //    }

            //}
            //SaveGameInfo2Disk();
        }

       
    }
}
