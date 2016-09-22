using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RaceManagerTool.Tools
{
    class XMLSerializeUitl
    {
        public static void Object2Xml<T>(T item, string FilePath)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            using (StreamWriter wr = new StreamWriter(FilePath))
            {
                xs.Serialize(wr, item);
            }
        }

        public static T Xml2Object<T>(string FilePath)
        {
            XmlSerializer xz = new XmlSerializer(typeof(T));
            using (XmlReader reader = XmlReader.Create(FilePath))
            {
                return (T)xz.Deserialize(reader);
            }
        }
    }
}
