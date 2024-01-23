using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RaceManagerTool.Models
{
    [Serializable]
    public class Player : BindableBase
    {
        
        public string QQ {get;set;}
        public string Name { get; set; }
        public bool Status { get; set; }

        [NonSerialized]
        private List<Group> groups;
        [XmlIgnore]
        public List<Group> Groups 
        {
            get { return groups; }
            set { groups = value; }
        }

        public int Win { get; set; }

        public int Tie { get; set; }
        public int Lose { get; set; }

        public int WinTurn { get; set;}

        private int tieTurn;
        public int TieTurn
        {
            get { return tieTurn; }
            set
            {
                tieTurn = value;
                RaisePropertyChanged("Tie");
            }
        }
        public int LoseTurn { get; set; }

        /// <summary>
        /// 总积分：所有轮次的积分和
        /// </summary>
        public int TotalPoints { get; set; } 

        /// <summary>
        /// 累进分：每轮比赛后的总积分逐一相加的总和
        /// </summary>
        public int ProgressivePoints { get; set; } 

        /// <summary>
        /// 中间分：去掉一个最高分和一个最低分后的总积分
        /// </summary>
        public int MiddlePoints { get; set; } 

        /// <summary>
        /// 对手分：所有对手的总积分和
        /// </summary>
        public int RivalTotalPoints { get; set;}

        /// <summary>
        /// //输赢差：胜负小局数差
        /// </summary>
        public int WinLoseDifference { get; set; } 

        /// <summary>
        /// 最高分：所有对手中总积分最高一位的总积分
        /// </summary>
        public int RivalHighestPoints { get; set; } 

        /// <summary>
        /// 排名
        /// </summary>
        public int Ranking { get; set; } 

        /// <summary>
        /// 进行的最终轮次序号
        /// </summary>
        public int FinalTurnIndex { get; set; }

        public Player()
        {
            Groups = new List<Group>();
        }
    }
}
