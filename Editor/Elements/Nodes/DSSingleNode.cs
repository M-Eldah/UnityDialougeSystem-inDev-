using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace DSystem.Elements
{
    using utilities;
    public class DSSingleNode : DialougeNode
    {
        public Port Choice;
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
           
            base.Initialize(Pos,graph);
            subType = SubType.SingleNode;
            AddToClassList("SingleNode");
            choices.Add("Dialouge");
        }
        public override void Initialize(Vector2 Pos, List<string> _Text, List<string> extra, GraphView graph)
        {

            base.Initialize(Pos,_Text, extra,graph);
            subType = SubType.SingleNode;
            AddToClassList("SingleNode");
            choices.Add("Dialouge");
        }
        public void Initialize(Vector2 Pos, List<string> _Text,int id, string name, List<int> cnodes, List<string> extra, GraphView graph, string _Tag)
        {
            Initialize(Pos,_Text, extra,graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            Tag = _Tag;
        }
        public override void DrawSingle()
        {
            base.DrawSingle();
            Choice = this.CreatePort("Output");
            Choice.portName = $"{Id}Output";
            outputContainer.Add(Choice);

            RefreshExpandedState();

        }

      
    }
}

