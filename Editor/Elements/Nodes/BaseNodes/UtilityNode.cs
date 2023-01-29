
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using UnityEditor.Experimental.GraphView;
    using utilities;
    public class UtilityNode : BaseNode
    {
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos,graph);
            mainType = NodeType.UtilityNode;
            choices = new List<string>();
            extraValues = new List<string>();
  
        }
        public override void Draw()
        {
            if (extraValues.Count == 0)
            {
                extraValues.Add(""); 
            }
            //input container
            base.Draw();
            //output container
            Port Choice = this.CreatePort("Output");
            Choice.portName = $"{Id}Output";
            output.Add(Choice);
            outputContainer.Add(Choice);
        }
    }
}


