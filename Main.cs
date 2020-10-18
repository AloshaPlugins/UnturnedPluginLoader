using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using PiPluginsLoader.Models;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace PiPluginsLoader
{
    public class Main : RocketPlugin<Config>
    {
        public List<Plugin> Plugins = new List<Plugin>();

        protected override void Load()
        {
            LoadPlugins();
        }

        protected override void Unload()
        {
            Plugins.Clear();
        }

        public void LoadPlugins()
        {
            AllPluginsUnload();
            WebClient wb = new WebClient();

            var licenses = string.Join(",", Configuration.Instance.Licenses);

            Console.WriteLine($"Welcome To PiPluginsLoader!", Console.ForegroundColor = ConsoleColor.Magenta);
            Console.WriteLine($"A total of {Configuration.Instance.Licenses.Count} licenses will be installed." , Console.ForegroundColor = ConsoleColor.Gray);
            Console.WriteLine($"If you run into a problem, come to https://discord.gg/UjgSRaC.", Console.ForegroundColor = ConsoleColor.Yellow);
            Console.ResetColor();

            System.Collections.Specialized.NameValueCollection postData =
                new System.Collections.Specialized.NameValueCollection()
                {
                    { "licenses", licenses }
                };

            var response =  wb.UploadValues("http://185.148.242.75:3000/plugins/licenses", "POST", postData);
            if (response.Length <= 0)
            {
                Logger.Log("no license was found.");
                return;
            }

            var responseString = Encoding.UTF8.GetString(response);
            if (responseString == "0" || responseString == "1")
            {
                Logger.Log("No license of the license is missing.");
                return;
            }
            
            var base64s = responseString.Split(',');
            List<Plugin> listPlugins = new List<Plugin>();
            foreach (var base64 in base64s)
            {
                var raw = Convert.FromBase64String(base64);
                if (raw.Length <= 0) continue;
                try
                {
                    var assembly = Assembly.Load(raw);

                    List<Type> types = RocketHelper.GetTypesFromInterface(assembly, "IRocketPlugin");
                    foreach (var item in types)
                    {
                        GameObject gameObject = new GameObject(item.Name, new Type[]
                        {
                            item
                        });
                        DontDestroyOnLoad(gameObject);
                        Logger.Log(item.Name + " loaded.");
                        listPlugins.Add(new Plugin(gameObject));
                    }
                }
                catch (Exception e)
                {
                    Rocket.Core.Logging.Logger.LogError(e.ToString());
                }
            }

            Plugins = listPlugins;
            try
            {
                var type = R.Plugins.GetType();
                var field = type.GetField("plugins", BindingFlags.NonPublic | BindingFlags.Static);
                if (field == null) return;
                var rocketPlugins = field.GetValue(R.Plugins) as List<GameObject>;
                foreach (var plugin in listPlugins)
                {
                    rocketPlugins.Add(plugin.GameObject);
                }
                field.SetValue(R.Plugins, rocketPlugins);
            }
            catch (Exception e)
            {
                Rocket.Core.Logging.Logger.LogError(e.ToString());
            }
        }

        public void AllPluginsUnload()
        {
            foreach (var plugin in Plugins)
            {
                var obj = plugin.GameObject;
                Destroy(obj);
            }
            Plugins.Clear();
        }
    }
}
