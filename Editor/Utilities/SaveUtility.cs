using System.Collections;
using System.Collections.Generic;
using DSystem;
using DSystem.Elements;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SaveUtility
{
    private static DSGraphView CurrentGraphview;
    private static DialougeData dialouge;
    
    public static List<NodeDB> nodes()
    {
        List<NodeDB> nodes = new List<NodeDB>();
        foreach(BaseNode node in CurrentGraphview.nodes.ToList())
        {
            Debug.Log(node.nodeName);
            NodeDB snodeDB=new NodeDB(node.nodeName,node.Id,node.dialougeText,node.extraValues,node.choices,node.GetPosition().position,node.mainType,node.connections,node.subType,node.q_string1,node.q_string2,node.q_bool1,node.q_bool2,node.Tag);
            nodes.Add( snodeDB);  
        }
        return nodes;
    }
    public static List<GroupsDB> Groups()
    {
        List<GroupsDB> Groups = new List<GroupsDB>();
        foreach (GraphElement ele in CurrentGraphview.graphElements)
        {
            if (ele is DSGroup group)
            {
                Groups.Add (new GroupsDB(group.GroupName,group.ContainedNodes,group.GetPosition().position));
               
            }
           
        }
        return Groups;
    }

    public static void Save(string DialougeName, DSGraphView graphView)
    {
        CurrentGraphview = graphView;
        dialouge = new DialougeData();
        dialouge.Nodes = nodes() ;
        dialouge.Group = Groups();
        dialouge.id = CurrentGraphview.id;
        dialouge.startIndex = CurrentGraphview.startid;
        dialouge.name = CurrentGraphview.DialougeName;
        string savefile = $"Assets/Dialouge/Resources/DialougesData/{DialougeName}.json";
        string jsondata = JsonConvert.SerializeObject(dialouge, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(savefile, jsondata);
    }

    public static DialougeData load(string DialougeName)
    {
        string savefile = $"Assets/Dialouge/Resources/DialougesData/{DialougeName}";
        if (File.Exists(savefile))
        {
            string JsonData = File.ReadAllText(savefile);
            dialouge = JsonConvert.DeserializeObject<DialougeData>(JsonData);
            return dialouge;
        }
        else
        {
            return null;
        }
       
    }
}
