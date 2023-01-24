using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Dialouge
{
    public string Text;
    public bool DialougeLocked;

    public Dialouge(string text, bool dialougeLocked)
    {
        Text = text;
        DialougeLocked = dialougeLocked;
    }
}

public class DsData
{
    public Dialouge Dialouge;
    public List<Dialouge> Choices;
    public Sprite Face;
    public bool Pause;
    public TextType type;
    public string Tag;
    public AudioClip clip;
    //for utility nodes that have nothing to display
    public DsData(bool pause = false,string tag = "", AudioClip _clip = null)
    {
        type = TextType.EmptyNode;
        Tag = tag;
        Pause = pause;
        clip = _clip;
        if (Pause == false)
        {
            Debug.Log(Pause);
            DialogSystem.Next();
        }
    }

    //
    public DsData(bool pause = false, string Dialogue = null, bool Extra = false, string tag = "")
    {
        Tag = tag;
        Pause = pause;
        string text = Dialogue;
        bool dialougeLocked = Extra;
        Dialouge = new Dialouge(text, dialougeLocked);
        type = TextType.SingleNode;
    }

    //for nodes with multiple choices
    public DsData(bool pause = false, List<string> Dialogue = null, List<bool> Extra = null, string tag = "")
    {
        Tag = tag;
        Pause = pause;
        Choices = new List<Dialouge>();
        for (int i = 0; i < Dialogue.Count; i++)
        {
            Choices.Add(new Dialouge(Dialogue[i], Extra[i]));
        }
        type = TextType.MultiNode;
    }
}

public static class DialogSystem
{
    public delegate void DialougeEnd();

    public static event DialougeEnd dialougeEnd;

    public delegate void DialougeNext();

    public static event DialougeNext dialougeNext;

    private static DialougeData data;
    private static Dictionary<int, NodeDB> Nodes;
    public static bool InDialouge;
    public static int currentindex;
    public static Dictionary<string, DialougeRecord> Records;

    public static void Next()
    {
        dialougeNext();
    }

    public static void End()
    {
        dialougeEnd();
    }

    public static DsData DStart(DialougeData _data)
    {
        load();
        data = _data;
        Nodes = nodeDictionary(data.Nodes);
        InDialouge = true;
        currentindex = Records[data.name].startindex;
        return NodeDataReturn();
    }

    public static DsData DNext(int index = 0)
    {
        //get the next node index or in case of single nodes check if all comments are read
        if (Nodes[currentindex].subType == SubType.SingleNode)
        {
            if (Nodes[currentindex].CommentIndex == Nodes[currentindex].dialougeText.Count - 1)
            {
                currentindex = Nodes[currentindex].ConnectedNodes[index];
            }
            else
            {
                Nodes[currentindex].CommentIndex++;
            }
        }
        else
        {
            currentindex = Nodes[currentindex].ConnectedNodes[index];
        }

        if (currentindex == -1)
        {
            Debug.Log("END");
            dialougeEnd();
            InDialouge = false;
            return null;
        }
        return NodeDataReturn();
    }

    private static DsData NodeDataReturn()
    {
        bool pause = NodeAction();
        if (Nodes[currentindex].NodeType == NodeType.UtilityNode || Nodes[currentindex].subType == SubType.ValueDirectionNode)
        {
            AudioClip clip = null;
            if(Nodes[currentindex].subType==SubType.AudioNode)
            {
                clip = (AudioClip)Resources.Load(Nodes[currentindex].q_string1);
            }
            return new DsData(pause, Nodes[currentindex].Tag,clip);
        }
        if (Nodes[currentindex].subType == SubType.SingleNode || Nodes[currentindex].subType == SubType.RandomNode || Nodes[currentindex].subType == SubType.MRandomNode)
        {
            CheckModifiedData();
            int Extraid = Nodes[currentindex].CommentIndex;
            bool locked;
            if (Nodes[currentindex].subType == SubType.MRandomNode)
            {
                locked = bool.Parse(Nodes[currentindex].extraValues[(Extraid * 3) + 6]);
            }
            else
            {
                locked = bool.Parse(Nodes[currentindex].extraValues[(Extraid * 3) + 2]);
            }
            return new DsData(false, Nodes[currentindex].dialougeText[Extraid], locked, Nodes[currentindex].Tag);
        }
        return new DsData(false, Nodes[currentindex].choices, UnlockedList(Nodes[currentindex].subType, Nodes[currentindex].extraValues), Nodes[currentindex].Tag);
    }
    private static bool NodeAction()
    {
        bool pause = false;
        switch (Nodes[currentindex].subType)
        {
            /// <summary>
            /// Set the field or modify it according to the value Extra[0]
            /// </summary>
            #region PropertychnageNode
            case SubType.Propertychangenode:
                if (!Nodes[currentindex].q_bool1)
                {
                    GameObject gameObject3 = GameObject.Find(Nodes[currentindex].q_string1);
                    List<PropertyInfo> Properties = UtilityFunctions.GetProperties(gameObject3);
                    foreach (PropertyInfo _m in Properties)
                    {
                        if (_m.Name == Nodes[currentindex].q_string2)
                        {
                            if (Nodes[currentindex].q_bool2)
                            {
                                if (_m.PropertyType == typeof(bool))
                                {
                                    Debug.LogError("You can't add Booleans");
                                }
                                else if (_m.PropertyType == typeof(int))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (int)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (int)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                                else if (_m.PropertyType == typeof(float))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (float)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (float)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                                else if (_m.PropertyType == typeof(string))
                                {
                                    _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), (string)Value(gameObject3, _m.Name, Nodes[currentindex].q_bool1) + (string)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                                }
                            }
                            else
                            {
                                _m.SetValue(gameObject3.GetComponent(UtilityFunctions.Type(gameObject3, _m)), Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.PropertyType));
                            }
                        }
                    }
                }
                else
                {
                    GameObject gameObject1 = GameObject.Find(Nodes[currentindex].q_string1);
                    List<FieldInfo> Properties = UtilityFunctions.GetFields(gameObject1);
                    foreach (FieldInfo _m in Properties)
                    {
                        if (_m.Name == Nodes[currentindex].q_string2)
                        {
                            if (Nodes[currentindex].q_bool2)
                            {
                                var nas = Value(gameObject1, _m.Name, Nodes[currentindex].q_bool1);
                                if (_m.FieldType == typeof(bool))
                                {
                                    Debug.LogError("You can't add Booleans");
                                }
                                else if (_m.FieldType == typeof(int))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (int)nas + (int)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                                else if (_m.FieldType == typeof(float))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (float)nas + (float)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                                else if (_m.FieldType == typeof(string))
                                {
                                    _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), (string)nas + (string)Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                                }
                            }
                            else
                            {
                                _m.SetValue(gameObject1.GetComponent(UtilityFunctions.Type(gameObject1, _m)), Convert.ChangeType(Nodes[currentindex].extraValues[0], _m.FieldType));
                            }
                        }
                    }
                }
                break;
            #endregion
            #region ActionNode
            case SubType.ActionNode:
                if (!Nodes[currentindex].q_bool2)
                {
                    GameObject gameObject2 = GameObject.Find(Nodes[currentindex].q_string1);
                    gameObject2.SendMessage(Nodes[currentindex].q_string2);
                }
                else
                {
                    GameObject gameObject2 = GameObject.Find(Nodes[currentindex].q_string1);
                    List<MethodInfo> methods = UtilityFunctions.GetMethods(gameObject2);

                    MethodInfo m = methods[0];
                    foreach (MethodInfo _m in methods)
                    {
                        if (_m.Name == Nodes[currentindex].q_string2)
                        {
                            m = _m;
                        }
                    }
                    ParameterInfo[] ps = m.GetParameters();
                    gameObject2.SendMessage(Nodes[currentindex].q_string2, Convert.ChangeType(Nodes[currentindex].extraValues[0], ps[0].ParameterType));
                }
                pause = Nodes[currentindex].q_bool1;
                break;
            #endregion
            #region RandomNodes
            case SubType.RandomNode:
                Nodes[currentindex].CommentIndex = UnityEngine.Random.Range(0, Nodes[currentindex].dialougeText.Count);
                break;

            case SubType.MRandomNode:
                bool valuetype = bool.Parse(Nodes[currentindex].extraValues[0]);
                bool greater = bool.Parse(Nodes[currentindex].extraValues[1]);
                GameObject gameObject = GameObject.Find(Nodes[currentindex].extraValues[2]);
                var ran = Value(gameObject, Nodes[currentindex].extraValues[3], valuetype);
                Debug.Log(ran);
                if (greater)
                {
                    Nodes[currentindex].CommentIndex = UnityEngine.Random.Range(Mathf.FloorToInt((float)ran), Nodes[currentindex].dialougeText.Count);
                }
                else
                {
                    Nodes[currentindex].CommentIndex = UnityEngine.Random.Range(0, Nodes[currentindex].dialougeText.Count - Mathf.FloorToInt((float)ran));
                }
                break;
            #endregion
            #region ChoiceUnlockNode
            case SubType.ChoiceUnlockNode:
                int node = int.Parse(Nodes[currentindex].q_string2);
                if (Nodes[currentindex].q_bool2)
                {
                    for (int i = int.Parse(Nodes[currentindex].extraValues[0]); i < int.Parse(Nodes[currentindex].extraValues[1]); i++)
                    {
                        int choice = i;
                        if (Records[Nodes[currentindex].q_string1.Split(".")[0]].modified.ContainsKey($"{node},{choice}") == false)
                        { Records[Nodes[currentindex].q_string1.Split(".")[0]].modified.Add($"{node},{choice}", Nodes[currentindex].q_bool1); }
                    }
                }
                else
                {
                    for (int i = int.Parse(Nodes[currentindex].extraValues[0]); i < int.Parse(Nodes[currentindex].extraValues[1]); i++)
                    {
                        int choice = (i * 3) + 2;
                        if (Records[Nodes[currentindex].q_string1.Split(".")[0]].modified.ContainsKey($"{node},{choice}") == false)
                        { Records[Nodes[currentindex].q_string1.Split(".")[0]].modified.Add($"{node},{choice}", Nodes[currentindex].q_bool1); }
                    }
                }
                Save();
                break;
            #endregion
            #region StartChangeNode
            case SubType.StartChangeNode:
                Debug.Log("TouchDown");
                Records[Nodes[currentindex].q_string1.Split(".")[0]].startindex = int.Parse(Nodes[currentindex].q_string2);
                Save();
                break;
            #endregion
            #region ValueDirectionNode
            case SubType.ValueDirectionNode:
                bool valuetype1 = bool.Parse(Nodes[currentindex].extraValues[0]);
                bool greater1 = bool.Parse(Nodes[currentindex].extraValues[1]);
                GameObject q_string1ect2 = GameObject.Find(Nodes[currentindex].extraValues[2]);
                var Tan = Value(q_string1ect2, Nodes[currentindex].extraValues[3], valuetype1);
                Debug.Log(greater1);
                Debug.Log(Nodes[currentindex].extraValues[1]);
                for (int i = 0; i < Nodes[currentindex].choices.Count; i++)
                {
                    if (greater1)
                    {
                        if (float.Parse(Nodes[currentindex].choices[i]) >= float.Parse(Tan.ToString()))
                        {
                            Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                            break;
                        }
                    }
                    else
                    {
                        if (float.Parse(Nodes[currentindex].choices[i]) <= float.Parse(Tan.ToString()))
                        {
                            Nodes[currentindex].ConnectedNodes[0] = Nodes[currentindex].ConnectedNodes[i];
                            break;
                        }
                    }
                }
                break;
            #endregion
            case SubType.AudioNode:
                pause = Nodes[currentindex].q_bool1;
                break;
        }
        return pause;
    }

    /// <summary>
    /// Used to retrive the value of needed by some of the ActionNodes
    /// </summary>
    /// <param name="GameObject"></param>
    /// <param name="valuename"></param>
    /// <returns></returns>
    private static object Value(GameObject gameObject, string valuename, bool type)
    {
        var value = new object();

        if (type)
        {
            List<FieldInfo> Properties = UtilityFunctions.GetFields(gameObject);
            foreach (FieldInfo _m in Properties)
            {
                //Debug.Log(_m.Name);
                if (_m.Name == valuename)
                {
                    Debug.Log("field");
                    value = _m.GetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, _m)));
                }
            }
        }
        else
        {
            List<PropertyInfo> Properties = UtilityFunctions.GetProperties(gameObject);
            foreach (PropertyInfo _m in Properties)
            {
                Debug.Log(_m.Name);
                if (_m.Name == valuename)
                {
                    Debug.Log("Propery");
                    value = _m.GetValue(gameObject.GetComponent(UtilityFunctions.Type(gameObject, _m)));
                }
            }
        }
        Debug.Log(value);
        return value;
    }

    /// <summary>
    /// Creating the boolean list used for choice noodes
    /// </summary>
    private static List<bool> UnlockedList(SubType type, List<string> Extra)
    {
        List<bool> Unlocks = new List<bool>();
        switch (type)
        {
            case SubType.MultiNode:
                CheckModifiedData();
                for (int i = 0; i < Extra.Count; i++)
                {
                    Unlocks.Add(bool.Parse(Extra[i]));
                }
                break;

            case SubType.ValueChoiceNode:
                bool valuetype = bool.Parse(Nodes[currentindex].extraValues[0]);
                bool greater = bool.Parse(Nodes[currentindex].extraValues[1]);
                GameObject gameObject = GameObject.Find(Nodes[currentindex].extraValues[2]);
                var ran = Value(gameObject, Nodes[currentindex].extraValues[3], valuetype);
                float limit = float.Parse(ran.ToString());
                if (greater)
                {
                    for (int i = 4; i < Extra.Count; i++)
                    {
                        if (float.Parse(Extra[i]) >= limit)
                        { Unlocks.Add(false); }
                        else
                        {
                            Unlocks.Add(true);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i > Extra.Count; i++)
                    {
                        if (float.Parse(Extra[i + 4]) >= limit)
                        { Unlocks.Add(false); }
                        else
                        {
                            Unlocks.Add(true);
                        }
                    }
                }

                break;
        }
        return Unlocks;
    }

    private static void CheckModifiedData()
    {
        switch (Nodes[currentindex].subType)
        {
            case SubType.MultiNode:
                for (int i = 0; i < Nodes[currentindex].extraValues.Count; i++)
                {
                    if (Records[data.name].modified.ContainsKey($"{currentindex},{i}"))
                    {
                        Nodes[currentindex].extraValues[i] = Records[data.name].modified[$"{currentindex},{i}"].ToString();
                    }
                }
                break;

            case SubType.SingleNode:
                int nodeid = (Nodes[currentindex].CommentIndex * 3) + 2;
                if (Records[data.name].modified.ContainsKey($"{currentindex},{nodeid}"))
                {
                    Nodes[currentindex].extraValues[nodeid] = Records[data.name].modified[$"{currentindex},{nodeid}"].ToString();
                }
                break;
        }
    }

    public static void Save()
    {
        Debug.Log("TouchDown");
        string savefile = $"{Application.persistentDataPath}/DialougeRecord.json";
        string jsondata = JsonConvert.SerializeObject(Records, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(savefile, jsondata);
    }

    public static void load()
    {
        string savefile = $"{Application.persistentDataPath}/DialougeRecord.json";
        var Dialouges = Resources.LoadAll<TextAsset>("DialougesData"); ;
        if (File.Exists(savefile))
        {
            string JsonData = File.ReadAllText(savefile);
            Records = JsonConvert.DeserializeObject<Dictionary<string, DialougeRecord>>(JsonData);
            if (Records.Count < Dialouges.Length)
            {
                foreach (TextAsset d in Dialouges)
                {
                    DialougeData dialouge = JsonConvert.DeserializeObject<DialougeData>(d.text);
                    if (!Records.ContainsKey(dialouge.name))
                    { Records.Add(dialouge.name, new DialougeRecord(dialouge.startIndex)); }
                }
            }
        }
        else
        {
            Records = new Dictionary<string, DialougeRecord>();
            Dialouges = Resources.LoadAll<TextAsset>("DialougesData"); ;
            foreach (TextAsset d in Dialouges)
            {
                DialougeData dialouge = JsonConvert.DeserializeObject<DialougeData>(d.text);
                Records.Add(dialouge.name, new DialougeRecord(dialouge.startIndex));
            }
            Save();
        }
    }

    public static Dictionary<int, NodeDB> nodeDictionary(List<NodeDB> nodes)
    {
        Dictionary<int, NodeDB> nodeD = new Dictionary<int, NodeDB>();
        foreach (NodeDB n in nodes)
        {
            nodeD.Add(n.id, n);
        }
        return nodeD;
    }
}