using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DSystem.Inspector
{

    [CustomEditor(typeof(DialougeHandeler))]
    public class DialougeInspector : Editor
    {
        string dialouge;
        public override void OnInspectorGUI()
        {   
            DialougeHandeler Dialouge = (DialougeHandeler)target;
            Dialouge.startingNode = EditorGUILayout.IntField("StartNode", Dialouge.startingNode);
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Assigned dialogue", "Which dialogue is this NPC going to own?"));
            Dialouge.index = EditorGUILayout.Popup(Dialouge.index, Dialougelist().ToArray());
            dialouge = Dialougelist().ToArray()[Dialouge.index];
            Dialouge.DialougeName = dialouge;
            GUILayout.EndHorizontal();
        }
        public List<string> Dialougelist()
        {
            List<string> DialougeList = new List<string>();
            DirectoryInfo di = new DirectoryInfo("Assets/Dialouge/Resources/DialougesData");
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var orderedFiles = files.OrderBy(f => f.CreationTimeUtc);
            foreach (FileSystemInfo d in orderedFiles.ToArray())
            {
                if (d.Extension == ".json")
                {
                    DialougeList.Add(d.Name.Split(".")[0]); 
                }
            }
            return DialougeList;
        }
    }
}