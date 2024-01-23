using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Mvvm;
using RaceManagerTool.Models;
using System.Xml.Serialization;

namespace RaceManagerTool.Models
{
    [Serializable]
    public class Group : BindableBase ,IDisposable
    {
        public int TurnIndex { get; set; }
        public int Num { get; set; }


        public string Q1 { get; set; }

        [NonSerialized]
        private Player play1;
        [XmlIgnore]
        public Player Play1 
        {
            get
            {
                return play1;
            }
            set
            {
                play1 = value;
                
                Q1 = value != null ? value.QQ : "";
            } 
        }

        public string Q2 { get; set; }
        

        [NonSerialized]
        private Player play2;
        [XmlIgnore]
        public Player Play2 
        {
            get
            {
                return play2;
            }
            set
            {
                play2 = value;
                Q2 = value != null ? value.QQ : "";
            } 
        }

        public String Remark { get; set; }

        public int Resultindex{ get; set; }

        [NonSerialized]
        private Result result;
        [XmlIgnore]
        public Result Result
        {
            get { return result; }
            set
            {
                result = value;
                RaisePropertyChanged("Result");
                Resultindex = value != null ? value.Index : -1;
            }
        }

        public Group()
        {
            Resultindex = -1;
        }

        public void Dispose()
        {
            Play1 = null;
            Play2 = null;
            Result = null;
        }

    }
}
