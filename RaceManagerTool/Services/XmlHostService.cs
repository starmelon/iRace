using RaceManagerTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RaceManagerTool.Services
{
    class XmlHostService
    {
        private static readonly XmlHostService instance = new XmlHostService();

        public List<Host> Host { get; set; }

        private XmlHostService()
        {
            Host = GetAllHosts();
        }

        public static XmlHostService GetInstance()
        {
            return instance;
        }

        public List<Host> GetAllHosts()
        {
            List<Host> hostsList = new List<Host>();
            string xmlFileName = System.IO.Path.Combine(Environment.CurrentDirectory, @"Data\Hosts.xml");
            XDocument xDoc = XDocument.Load(xmlFileName);
            var hosts = xDoc.Descendants("Host");
            foreach (var r in hosts)
            {
                Host host = new Host();
                host.QQ = r.Element("QQ").Value;
                host.Name = r.Element("Name").Value;
                host.Introduction = r.Element("Introduction").Value;
                hostsList.Add(host);
            }

            return hostsList;
        }
    }
}
