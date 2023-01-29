using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;

    public class BaseNode : Node
    {
        
        public List<Port> output = new List<Port>();
        public Vector2 pos;
        public string nodeName;
        public int groupid;
        public int Id;
        public string Tag;
        public Port inputport;
        public NodeType mainType;
        public SubType subType;
        public List<int> connections;
        //Choice
        public List<string> choices { get; set; }
        //Text
        public List<string> dialougeText { get; set; }
        public List<string> extraValues { get; set; }
        //public List<string> Extra { get; set; }
        //q_string1ect
        public string q_string1 { get; set; }
        //Method
        public string q_string2 { get; set; }
        //pause
        public bool q_bool1 { get; set; }
        //hasParameter
        public bool q_bool2 { get; set; }
        protected GraphView GraphView;
        public BaseNode()
        {
            connections = new List<int>();
        }

        public virtual void Initialize(Vector2 Pos, GraphView graph)
        {
            GraphView = graph;
            pos = Pos;
            SetPosition(new Rect(Pos, Vector2.zero));
        }

        public virtual void Draw()
        {
            //input container
            inputport = this.CreatePort("Input", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputport.portName = $"{Id}Input";
            inputContainer.Add(inputport);
            TextField tag = DSElementUtilities.CreateTextField("Tag", evt => Tag = evt.newValue);
            if(Tag!=null)
            {
                tag.value = Tag;
            }
            tag.label = "Tag";
            extensionContainer.Add(tag);
        }

        public void SaveConnections()
        {
            connections.Clear();
            foreach (Port port in output)
            {
                if (!port.connected)
                {
                    connections.Add(-1);
                    continue;
                }
                List<Edge> edges = new List<Edge>(port.connections);
                BaseNode node = (BaseNode)edges[0].input.node;
                connections.Add(node.Id);
            }
        }

        public List<Edge> DisconnectPorts()
        {
            List<Edge> edges = new List<Edge>();
            foreach (Port port in output)
            {
                edges.AddRange(port.connections);
            }
            foreach (Port port in inputContainer.Children())
            {
                edges.AddRange(port.connections);
            }
            return edges;
        }

        public List<Edge> nodeConnect(Dictionary<int, BaseNode> Nodes)
        {
            List<Edge> edge = new List<Edge>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i] != -1)
                {
                    edge.Add(output[i].ConnectTo(Nodes[connections[i]].inputport));
                }
            }
            return edge;
        }

        public void DisconnectPorts(VisualElement container, DSGraphView graphView)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }
    }
}