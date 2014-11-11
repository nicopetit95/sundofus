using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace SunDofus.Utilities
{
    public class Config
    {
        public string RealmAddress { get; set; }
        public int RealmPort { get; set; }
        public string GameAddress { get; set; }
        public int GamePort { get; set; }
        public int GameID { get; set; }
        public int MaxPlayer { get; set; }
        public string DBServer { get; set; }
        public string DBUser { get; set; }
        public string DBPass { get; set; }
        public string DBName { get; set; }

        public static Config Get()
        {
            FileStream readFileStream = new FileStream("settings.xml", FileMode.Open, FileAccess.Read, FileShare.Read);

            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            Config loadedConf = (Config)serializer.Deserialize(readFileStream);

            readFileStream.Close();

            return loadedConf;
        }

        public void SaveFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            TextWriter writeFileStream = new StreamWriter("settings.xml");
            serializer.Serialize(writeFileStream, this);

            writeFileStream.Close();
        }

        private static string version = string.Empty;

        public static string Version(string nVersion = "")
        {
            if (version.Equals(string.Empty))
                version = nVersion;

            return version;
        }
    }
}
