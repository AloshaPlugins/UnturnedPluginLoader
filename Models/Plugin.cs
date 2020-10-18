using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PiPluginsLoader.Models
{
    public class Plugin
    {
        public GameObject GameObject;

        public Plugin() { }

        public Plugin(GameObject gameObject)
        {
            this.GameObject = gameObject;
        }
    }
}
