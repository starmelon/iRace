using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.Models
{
    [Serializable]
    public class GameSetting:BindableBase,IEditableObject
    {
        struct GameData
        {
            internal string name;//比赛名字
            internal string fullname;//比赛全称
            internal string gametype;//比赛类型
            internal int num;//额定人数
            internal int realnum;//实际参赛人数
            internal int turns;//回合数
            internal Host host;//主持人
        }

        private GameData curData;//最新的数据
        private GameData backupData;//上一次保存的数据
        private bool inTxn = false;//类的可编辑状态

         void IEditableObject.BeginEdit()
        {
            Console.WriteLine("Start Game BeginEdit");
            if (!inTxn)
            {
                this.backupData = curData;
                inTxn = true;
            }
        }

        void IEditableObject.EndEdit()
        {
            Console.WriteLine("Start Game EndEdit");
            if (inTxn)
            {
                backupData = new GameData();
                inTxn = false;
            }
        }

        void IEditableObject.CancelEdit()
        {
            Console.WriteLine("Start Game CancelEdit");
            if (inTxn)
            {
                this.curData = backupData;
                OnAllPropertyChanged();
                inTxn = false;
            }


        }

        private void OnAllPropertyChanged()
        {
            RaisePropertyChanged("Name");
            RaisePropertyChanged("Fullname");
            RaisePropertyChanged("GameType");
            RaisePropertyChanged("Num");
            RaisePropertyChanged("RealNum");
            RaisePropertyChanged("Turns");
            RaisePropertyChanged("Host");
        }

        public string Name
        {
            get
            {
                return this.curData.name;
            }
            set
            {
                this.curData.name = value;
                RaisePropertyChanged("Name");
                setFullName();
            }
        }

        private void setFullName()
        {
            FullName = Name + " - [" + (GameType != null ? GameType : "") + "][" + Turns + "轮][" + +Num + "人]" + "[主持@" + (Host != null ? Host.Name : "") + "]";
        }

        public string FullNameOld
        {
            get
            {
                return this.backupData.fullname;
            }
        }

        public string FullName 
        {
            get
            {
                return this.curData.fullname;
            }
            set
            {
                this.curData.fullname = value;
                RaisePropertyChanged("Fullname");
            }
        }

        public string GameType
        {
            get
            {
                return this.curData.gametype;
            }
            set
            {
                this.curData.gametype = value;
                RaisePropertyChanged("GameType");
                setFullName();
            }
        }

        public int NumOld
        {
            get
            {
                return this.backupData.num;
            }
        }

        public int Num
        {
            get
            {
                return this.curData.num;
            }
            set
            {
                this.curData.num = value;
                RaisePropertyChanged("Num");
                setFullName();
            }
        }
        
        public int RealNum
        {
            get
            {
                return this.curData.realnum;
            }
            set
            {
                this.curData.realnum = value;
                RaisePropertyChanged("RealNum");
            }
        }

        public int Turns
        {
            get
            {
                return this.curData.turns;
            }
            set
            {
                this.curData.turns = value;
                RaisePropertyChanged("Turns");
                setFullName();
            }
        }
        public Host Host
        {
            get
            {
                return this.curData.host;
            }
            set
            {
                this.curData.host = value;
                RaisePropertyChanged("Host");
                setFullName();
            }
        }




    }
}
