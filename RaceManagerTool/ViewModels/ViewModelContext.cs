using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceManagerTool.ViewModels
{
    
    class ViewModelContext
    {
        public static readonly object ShowTurns = new object(); //更新要显示的轮次
        public static readonly object UpdataRank = new object(); //更新排名
        public static readonly object SetGameType = new object(); //设置比赛类型
        public static readonly object CleanGame = new object(); //清空当前比赛
        public static readonly object GameInfoChange = new object(); //卡组拖放添加玩家
    }
}
