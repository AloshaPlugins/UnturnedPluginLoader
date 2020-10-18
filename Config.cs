using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Rocket.API;
using SDG.Framework.Debug;

namespace PiPluginsLoader
{
    public class Config : IRocketPluginConfiguration
    {
        [XmlArray("Licenses")]
        [XmlArrayItem("License")]
        public List<string> Licenses = new List<string>();
        public void LoadDefaults()
        {
            Licenses = new List<string>()
            {
                "xxxxx-xxxxx-xxxxx",
                "xxxxx-xxxxx-xxxxx-2"
            };
        }
    }
}
