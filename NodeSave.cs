using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//The class used to save the Dialouge
public class DialougeData
{
    public string name;
    public int id;
    public int startIndex;
    public List<NodeDB> Nodes;
    public List<GroupsDB> Group;
    public DialougeData()
    {
        Group = new List<GroupsDB>();
    }
}
//The class used to save the Nodes
public class NodeDB
{
    //and index we use to check which part of the dialouge we are on
    public int CommentIndex;

    public string name;

    public int id;

    public List<string> dialougeText;

    public List<string> extraValues;

    public List<string> choices;
    //the position of the node on the grapht
    public Vector2 pos;
    //explained in NodesTypes.cs
    public NodeType NodeType;
    //The listof Nodes he has connections with,
    public List<int> ConnectedNodes;
    //Line 35
    public SubType subType;
    //for utilityNode

    public string q_string1;

    public string q_string2;
 
    public bool q_bool1;

    public bool q_bool2;

    //to deliver the extra string
    public string Tag;

    public NodeDB(string name, int id, List<string> dialougeText, List<string> extraValues, List<string> choices, Vector2 pos, NodeType nodeType, List<int> connectedNodes, SubType subType, string q_string1, string q_string2, bool q_bool1, bool q_bool2, string tag)
    {
        this.name = name;
        this.id = id;
        this.dialougeText = dialougeText;
        this.extraValues = extraValues;
        this.choices = choices;
        this.pos = pos;
        NodeType = nodeType;
        ConnectedNodes = connectedNodes;
        this.subType = subType;
        this.q_string1 = q_string1;
        this.q_string2 = q_string2;
        this.q_bool1 = q_bool1;
        this.q_bool2 = q_bool2;
        Tag = tag;
    }
    //Long Class Constructor goes brrrrrrrrrrrrrrrrrrr

}

//this is used to save Groups
//have you been using nodes to make your dialouge easier to navigate, you should
public class GroupsDB
{
    //I ran out comedy
    public string GroupName;
    //He can take on lots of nodes at once don't worry >0                                               
    public List<int> ContainedNodes;
    //Line 32
    public Vector2 Position;
    public GroupsDB(string name, List<int> _Cnodes, Vector2 pos)
    {
        GroupName = name;
        ContainedNodes = _Cnodes;
        Position = pos;
    }
}
