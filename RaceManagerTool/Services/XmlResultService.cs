using RaceManagerTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RaceManagerTool.Services
{
    class XmlResultService
    {
        private static readonly XmlResultService instance = new XmlResultService();

        private XmlResultService()
        {

        }

        public static XmlResultService GetInstance()
        {
            return instance;
        }


        public List<Result> GetDefaultResults()
        {
            string xmlFileName = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data\Results.xml");
            return GetAllResults(xmlFileName);
        }

        public List<Result> GetAllResults(string path)
        {
            List<Result> resultsList = new List<Result>();
            XDocument xDoc = XDocument.Load(path);
            var Results = xDoc.Descendants("Result");

            int index = 0;
            foreach (var r in Results)
            {
                Result result = new Result();
                result.Index = index;
                index++;
                result.Win = Convert.ToInt32(r.Element("Win").Value);
                result.Tie = Convert.ToInt32(r.Element("Tie").Value);
                result.Lose = Convert.ToInt32(r.Element("Lose").Value);
                result.Define = r.Element("Define").Value;
                result.PointLeft = Convert.ToInt32(r.Element("PointLeft").Value);
                result.PointRight = Convert.ToInt32(r.Element("PointRight").Value);
                resultsList.Add(result);
            }

            return resultsList;
        }
    }
}
