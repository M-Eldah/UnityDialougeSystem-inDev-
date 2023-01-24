using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Reflection;
using System;

namespace DSystem
{
    //the class which carries the dialogue data until it is needed
    public class DialougeHandeler : MonoBehaviour
    {
        public string DialougeName;
        public int index;
        public int startingNode;
        public int currentindex = 0;

        public DialougeData Data()
        {
            var savefile = Resources.Load<TextAsset>($"DialougesData/{DialougeName}");
            if (savefile != null)
            {
                return JsonConvert.DeserializeObject<DialougeData>(savefile.text);
            }
            return null;
        }
    }
}

