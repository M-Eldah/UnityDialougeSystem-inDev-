using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem
{
    using DSystem.Windows;
    using Elements;
    using System.Collections.Generic;
    [System.Serializable]
    public class DSGraphView : GraphView
    {
        private DialougeSystemWindow editorWindow;
        private DSSearchWindow searchWindow;

        private BaseNode lastNode;
        private bool shift;
        public int groupid;
        public int id;
        public int currentid;
        public int startid;
        public string DialougeName;
        private Dictionary<int, BaseNode> Nodes;

        public DSGraphView(DialougeSystemWindow window)
        {
            editorWindow = window;

            AddManipulators();
            RegCallbacks();
            AddGridBackground();
            AddSearchwindow();
            OndeleteElements();

            addStyles();
            Nodes = new Dictionary<int, BaseNode>();
        }

        #region OverRide Methods

        /// <summary>
        /// Making ports able to connect to other ports of diffrent direction
        /// </summary>
        /// <param name="startPort"></param>
        /// <param name="nodeAdapter"></param>
        /// <returns></returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> Combatiableport = new List<Port>();
            ports.ForEach(Port =>
            {
                if (startPort == Port)
                {
                    return;
                }
                if (startPort.node == Port.node)
                {
                    return;
                }
                if (startPort.direction == Port.direction)
                {
                    return;
                }
                Combatiableport.Add(Port);
            }
            );
            return Combatiableport;
        }

        #endregion OverRide Methods

        #region Elements,Manipulators and Keyboard callbacks

        /// <summary>
        /// Manipulators and callbacks used for zooming adding nodes, and registering keyboard Shortcuts
        /// </summary>
        private void AddManipulators()
        {
            SetupZoom(0.1f, 10,1,0.5f);
            this.AddManipulator(CreatecopyNode());
            this.AddManipulator(CreateNodeContextualmenu("Add (SingleNode)", NodeType.DialougeNode , SubType.SingleNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (MultiNode)", NodeType.DialougeNode  , SubType.MultiNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (RandomNode)", NodeType.DialougeNode  , SubType.RandomNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ModifiedRandomNode)", NodeType.DialougeNode, SubType.MRandomNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ValueChoiceNode)", NodeType.DialougeNode, SubType.ValueChoiceNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ValueDirectionNode)", NodeType.DialougeNode, SubType.ValueDirectionNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ActionNode)", NodeType.UtilityNode  , SubType.ActionNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (ChoiceUnlock)", NodeType.UtilityNode, SubType.ChoiceUnlockNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (PropertyChangeNode)", NodeType.UtilityNode, SubType.Propertychangenode));
            this.AddManipulator(CreateNodeContextualmenu("Add (OverwriteStartNode)", NodeType.UtilityNode, SubType.StartChangeNode));
            this.AddManipulator(CreateNodeContextualmenu("Add (AudioNode)", NodeType.UtilityNode, SubType.AudioNode));
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(CreateGropContextualMenu());
        }

        private void RegCallbacks()
        {
            this.RegisterCallback<KeyDownEvent>(Shiftpress);
            this.RegisterCallback<KeyUpEvent>(Shiftpress2);
            this.RegisterCallback<KeyDownEvent>(MyCallback);
        }

        private void Shiftpress(KeyDownEvent evt)
        {
            if (evt.keyCode == KeyCode.LeftShift)
            {
                shift = true;
            }
        }

        private void Shiftpress2(KeyUpEvent evt)
        {
            if (evt.keyCode == KeyCode.LeftShift)
            {
                shift = false;
            }
        }

        private void MyCallback(KeyDownEvent evt)
        { if (evt.keyCode == KeyCode.N && shift) { CreateNode(lastNode.mainType, lastNode.subType, lastNode.GetPosition().position + new Vector2(100, 0)); } }

        #region AddElements
        private IManipulator CreatecopyNode()
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Copynode", actionEvent => copySelection())
            );
            return contextualMenu;
        }
        private IManipulator CreateGropContextualMenu()
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => AddElement(Creategroup("DialougeGroup", GetLocalMousePosition(actionEvent.eventInfo.mousePosition))))
            );
            return contextualMenu;
        }

        public Group Creategroup(string Title, Vector2 vector2)
        {
            DSGroup group = new DSGroup()
            {
                title = Title
            };
            group.SetPosition(new Rect(vector2, Vector2.zero));
            foreach (GraphElement selected in selection)
            {
                if (!(selected is BaseNode))
                {
                    continue;
                }
                BaseNode node = (BaseNode)selected;
                group.AddElement(node);
            }
            return group;
        }

        private IManipulator CreateNodeContextualmenu(string actionTile, NodeType dialougeType, SubType subType)
        {
            ContextualMenuManipulator contextualMenu = new ContextualMenuManipulator
            (
                menuEvent => menuEvent.menu.AppendAction(actionTile, actionEvent => CreateNode(dialougeType, subType, GetLocalMousePosition(actionEvent.eventInfo.mousePosition)))
            );
            return contextualMenu;
        }

        /// <summary>
        /// For new Dialouge
        /// </summary>
        /// <param name="dialougeType"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void CreateNode(NodeType dialougeType, SubType subType, Vector2 pos)
        {
            switch (dialougeType)
            {
                case NodeType.DialougeNode:
                    Type nodeType = Type.GetType($"DSystem.Elements.DS{subType}");
                    DialougeNode node = (DialougeNode)Activator.CreateInstance(nodeType);
                    node.title = $"{subType}-ID:{id}";
                    node.nodeName = node.title;
                    node.Id = id;
                    id++;
                    node.Initialize(pos, this);
                    if(subType==SubType.SingleNode|| subType == SubType.RandomNode||subType==SubType.MRandomNode)
                    {
                        node.DrawSingle();
                    }
                    else
                    {
                        node.Draw();
                    }
                    AddElement(node);
                    if (selection.Count == 1)
                    {
                        if (selection[0] is DSGroup group)
                        {
                            group.AddElement(node);
                        }
                    }

                    Nodes.Add(node.Id, node);
                    lastNode = node;
                    break;

                case NodeType.UtilityNode:
                    Type UtilitynodeType = Type.GetType($"DSystem.Elements.DS{subType}");
                    UtilityNode Utilitynode = (UtilityNode)Activator.CreateInstance(UtilitynodeType);
                    Utilitynode.title = $"{subType}-ID:{id}";
                    Utilitynode.nodeName = Utilitynode.title;
                    Utilitynode.Id = id;
                    id++;
                    Utilitynode.Initialize(pos,this);
                    Utilitynode.Draw();
                    AddElement(Utilitynode);
                    if (selection.Count == 1)
                    {
                        if (selection[0] is DSGroup group)
                        {
                            group.AddElement(Utilitynode);
                        }
                    }
                    Nodes.Add(Utilitynode.Id, Utilitynode);
                    lastNode = Utilitynode;
                    break;

                default:
                    break;
            }
        }

        public void loadNode(List<string> _text, List<string> _c, Vector2 _pos, NodeType dialougeType, SubType subType, List<int> Nodelist, string _N = "Node", int _id = 0, List<string> extra = null, string _Tag = null, string _obj = "", string _Method = "", bool _pause = false, bool _hasParameter=false)
        {
            switch (dialougeType)
            {
                case NodeType.DialougeNode:
                    Type nodeType = Type.GetType($"DSystem.Elements.DS{subType}");

                    switch (subType)
                    {
                        case SubType.SingleNode:
                            DSSingleNode snode = (DSSingleNode)Activator.CreateInstance(nodeType);
                            snode.Initialize(_pos, _text, _id, _N, Nodelist,extra, this,_Tag);
                            snode.DrawSingle();
                            AddElement(snode);
                            Nodes.Add(snode.Id, snode);
                            lastNode = snode;
                            break;
                        case SubType.RandomNode:
                            DSRandomNode Rnode = (DSRandomNode)Activator.CreateInstance(nodeType);
                            Rnode.Initialize(_pos, _text, _id, _N, Nodelist, extra, this, _Tag);
                            Rnode.DrawSingle();
                            AddElement(Rnode);
                            Nodes.Add(Rnode.Id, Rnode);
                            lastNode = Rnode;
                            break;
                        case SubType.MRandomNode:
                            DSMRandomNode MRnode = (DSMRandomNode)Activator.CreateInstance(nodeType);
                            MRnode.Initialize(_pos, _text, _id, _N, Nodelist, extra, this, _Tag);
                            MRnode.DrawSingle();
                            AddElement(MRnode);
                            Nodes.Add(MRnode.Id, MRnode);
                            lastNode = MRnode;
                            break;

                        case SubType.MultiNode:
                            DSMultiNode lcnode = (DSMultiNode)Activator.CreateInstance(nodeType);
                            lcnode.Initialize(_pos, _c, _text, _id, _N, Nodelist, extra,this, _Tag);
                            lcnode.Draw();
                            AddElement(lcnode);
                            Nodes.Add(lcnode.Id, lcnode);
                            lastNode = lcnode;
                            break;
                        case SubType.ValueChoiceNode:
                            DSValueChoiceNode vcnode = (DSValueChoiceNode)Activator.CreateInstance(nodeType);
                            vcnode.Initialize(_pos, _c, _text, _id, _N, Nodelist, extra, this, _Tag);
                            vcnode.Draw();
                            AddElement(vcnode);
                            Nodes.Add(vcnode.Id, vcnode);
                            lastNode = vcnode;
                            break;
                        case SubType.ValueDirectionNode:
                            DSValueDirectionNode vdnode = (DSValueDirectionNode)Activator.CreateInstance(nodeType);
                            vdnode.Initialize(_pos, _c, _text, _id, _N, Nodelist, extra, this, _Tag);
                            vdnode.Draw();
                            AddElement(vdnode);
                            Nodes.Add(vdnode.Id, vdnode);
                            lastNode = vdnode;
                            break;
                    }
                    break;

                case NodeType.UtilityNode:
                    Type UtilitynodeType = Type.GetType($"DSystem.Elements.DS{subType}");
                    switch (subType)
                    {
                        case SubType.ActionNode:
                            DSActionNode Anode = (DSActionNode)Activator.CreateInstance(UtilitynodeType);
                            Anode.Initialize(_pos, _id, _N, Nodelist, _obj, _Method, _pause, extra,this, _Tag);
                            Anode.Draw();
                            AddElement(Anode);
                            Nodes.Add(Anode.Id, Anode);
                            lastNode = Anode;
                            break;
                        case SubType.Propertychangenode:
                            DSPropertychangenode pnode = (DSPropertychangenode)Activator.CreateInstance(UtilitynodeType);
                            pnode.Initialize(_pos, _id, _N, Nodelist, _obj, _Method, _pause, extra,this, _Tag);
                            pnode.Draw();
                            AddElement(pnode);
                            Nodes.Add(pnode.Id, pnode);
                            lastNode = pnode;
                            break;
                        case SubType.ChoiceUnlockNode:
                            DSChoiceUnlockNode cunode = (DSChoiceUnlockNode)Activator.CreateInstance(UtilitynodeType);
                            cunode.Initialize(_pos, _id, _N, Nodelist, _obj, _Method, _pause, _hasParameter, extra,this, _Tag);
                            cunode.Draw();
                            AddElement(cunode);
                            Nodes.Add(cunode.Id, cunode);
                            lastNode = cunode;
                            break;
                        case SubType.StartChangeNode:
                            DSStartChangeNode scnode = (DSStartChangeNode)Activator.CreateInstance(UtilitynodeType);
                            scnode.Initialize(_pos, _id, _N, Nodelist, _obj, _Method, _pause, extra,this, _Tag);
                            scnode.Draw();
                            AddElement(scnode);
                            Nodes.Add(scnode.Id, scnode);
                            lastNode = scnode;
                            break;
                        case SubType.AudioNode:
                            DSAudioNode Aunode = (DSAudioNode)Activator.CreateInstance(UtilitynodeType);
                            Aunode.Initialize(_pos, _id, _N, Nodelist, _obj, _Method, _pause, extra, this, _Tag);
                            Aunode.Draw();
                            AddElement(Aunode);
                            Nodes.Add(Aunode.Id, Aunode);
                            lastNode = Aunode;
                            break;
                    }
                    break;
                  

                default:
                    break;
            }
        }
        void loadGroup(string name, Vector2 pos, List<int>nodes)
        {
            DSGroup group = new DSGroup()
            {
                title = name
            };
            group.SetPosition(new Rect(pos, Vector2.zero));
            foreach (int node in nodes)
            {
                
                group.AddElement(Nodes[node]);
            }
            AddElement(group);
       
        }
        private void AddSearchwindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                searchWindow.Intialiaze(this);
            }
            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        #endregion AddElements

        #endregion Elements,Manipulators and Keyboard callbacks

        #region Utilities
        public void ClearGraph()
        {
            Nodes.Clear();
            id = 0;
            startid = 0;
            groupid = 0;
            DeleteElements(graphElements);
        }
        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool issearchwindow = false)
        {
            Vector2 worldMousePosition = mousePosition;
            if (issearchwindow)
            {
                worldMousePosition -= editorWindow.position.position;
            }
            Vector2 LocalMousepos = contentViewContainer.WorldToLocal(worldMousePosition);
            return LocalMousepos;
        }
        private void copySelection()
        {
            //TobeAdded
            /*foreach (GraphElement selected in selection)
            {
                
            }*/
        }
      
        private void OndeleteElements()
        {
            List<BaseNode> DeletedNodes = new List<BaseNode>();
            List<DSGroup> DeletedGroups = new List<DSGroup>();
            List<Edge> DeletedEdges = new List<Edge>();
            deleteSelection = (operationName, AskUser) =>
            {
                foreach (GraphElement ele in selection)
                {
                    if (ele is BaseNode node)
                    {
                        DeletedNodes.Add(node);
                        continue;
                    }
                    if (ele is Edge edge)
                    {
                        DeletedEdges.Add(edge);
                        continue;
                    }
                    if (ele is DSGroup group)
                    {
                        DeletedGroups.Add(group);
                    }
                }
                foreach (DSGroup group in DeletedGroups)
                {
                    foreach (int id in group.ContainedNodes)
                    {
                        group.RemoveElement(Nodes[id]);
                    }
                }

                foreach (BaseNode Node in DeletedNodes)
                {
                    Nodes.Remove(Node.Id);
                    DeletedEdges.AddRange(Node.DisconnectPorts());
                }
                DeleteElements(DeletedNodes);
                DeleteElements(DeletedGroups);
                DeleteElements(DeletedEdges);
                DeletedNodes.Clear();
                DeletedGroups.Clear();
                DeletedEdges.Clear();
            };
        }

        public void save(string dialougeName)
        {
            foreach (GraphElement ele in graphElements)
            {
                if (ele is BaseNode node)
                {
                    node.SaveConnections();
                }
                if (ele is DSGroup group)
                {
                    group.saveGroup();
                }
            }
            DialougeName = dialougeName;
            SaveUtility.Save(dialougeName,this);
        }

        public int LoadGraph(string dialougeName)
        {
            Nodes.Clear();
            DialougeData dialouge = SaveUtility.load(dialougeName);
            foreach (NodeDB node in dialouge.Nodes)
            {
                switch (node.NodeType)
                {
                    case NodeType.DialougeNode:
                        loadNode(node.dialougeText, node.choices, node.pos, node.NodeType, node.subType, node.ConnectedNodes, node.name, node.id, node.extraValues, node.Tag);
                        break;

                    case NodeType.UtilityNode:
                        loadNode(node.dialougeText, node.choices, node.pos, node.NodeType, node.subType, node.ConnectedNodes, node.name, node.id,node.extraValues, node.Tag,node.q_string1, node.q_string2,node.q_bool1,node.q_bool2);
                        break;
                }
            }

            foreach (BaseNode node in nodes.ToList())
            {
                List<Edge> edges = node.nodeConnect(Nodes);
                foreach (Edge edge in edges)
                {
                    AddElement(edge);
                }
            }

            foreach(GroupsDB groupDB in dialouge.Group)
            {
                loadGroup(groupDB.GroupName, groupDB.Position, groupDB.ContainedNodes);
            }
            id = dialouge.id;
            startid = dialouge.startIndex;
            DialougeName = dialouge.name;
            return startid;
        }

        #endregion Utilities
        // the refrence to where the styles files are
        private void addStyles()
        {
            StyleSheet GraphstyleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/Dialouge/Editor/StyleSheet/DsGraphStyle.uss");
            StyleSheet NodestyleSheet = (StyleSheet)EditorGUIUtility.Load("Assets/Dialouge/Editor/StyleSheet/DsNodeStyleSheet.uss");
            styleSheets.Add(GraphstyleSheet);
            styleSheets.Add(NodestyleSheet);
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();
            gridBackground.StretchToParentSize();
            Insert(0, gridBackground);
        }
    }
}