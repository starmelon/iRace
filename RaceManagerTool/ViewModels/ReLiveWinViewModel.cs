using Prism.Commands;
using Prism.Mvvm;
using RaceManagerTool.Models;
using RaceManagerTool.Services;
using RaceManagerTool.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace RaceManagerTool.ViewModels
{
    class ReLiveWinViewModel:BindableBase
    {
        public ObservableCollection<DeadPlayerViewModel> LosePlayers { get; set; }

        public DelegateCommand ReliveCommand { get; set; }
        public DelegateCommand OutputSelectPlayers { get; set; }
        public DelegateCommand SelectReliveCommand { get; set; }
        public DelegateCommand AutoReliveCommand { get; set; }

        public int ReliveNumMax { get; set; }

        private int reliveNum;
        public int ReliveNum
        {
            get { return reliveNum; }
            set
            {
                reliveNum = value;
                this.RaisePropertyChanged("ReliveNum");
            }
        }

        private bool artificialMode;
        public bool ArtificialMode
        {
            get { return artificialMode; }
            set
            {
                artificialMode = value;
                this.RaisePropertyChanged("ArtificialMode");
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
            ArtificialMode = true;
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
            this.AutoReliveCommand = new DelegateCommand(new Action(this.AutoReliveCommandExcute));

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


        private bool CanAutoReliveCommandExecute()
        {
            return true;
        }

        /// <summary>
        /// 随机复活
        /// </summary>
        private void AutoReliveCommandExcute()
        {
            ArtificialMode = false;

            foreach (var item in LosePlayers)
            {
                item.CanReLive = false;
            }

            Random random = new Random(GetRandomSeed());
            for (int i = 0; i < ReliveNumMax; i++)
            {
                while (true)
                {
                    int reliveindex = random.Next(0, LosePlayers.Count - 1);
                    if (LosePlayers[reliveindex].CanReLive == false)
                    {
                        LosePlayers[reliveindex].CanReLive = true;
                        break;
                    }
                }
                

            }

            ReliveNum = 3;

        }

        /// <summary>
        /// 使用RNGCryptoServiceProvider生成种子
        /// </summary>
        /// <returns></returns>
        static int GetRandomSeed()
        {

            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);

        }

        /// <summary>
        /// DataGridView跨越滚动条截图
        /// </summary>
        /// <param name="dgv">DataGridView</param>
        /// <returns>图形</returns>
        private static System.Drawing.Image GetDataGridView(DataGridView dgv)
        {
            PictureBox pic = new PictureBox();
            pic.Size = dgv.Size;
            pic.Location = dgv.Location;
            Bitmap bmpPre = new Bitmap(pic.Width, pic.Height);
            dgv.DrawToBitmap(bmpPre, new Rectangle(0, 0, pic.Width, pic.Height));
            pic.Image = bmpPre;
            dgv.Parent.Controls.Add(pic);

            dgv.Visible = false;
            dgv.AutoSize = true;

            Bitmap bmpNew = new Bitmap(dgv.Width, dgv.Height);

            dgv.DrawToBitmap(bmpNew, new Rectangle(0, 0, dgv.Width, dgv.Height));

            dgv.AutoSize = false;
            dgv.Visible = true;

            dgv.Parent.Controls.Remove(pic);
            bmpPre.Dispose();
            pic.Dispose();
            return bmpNew;
        }
    }
}
