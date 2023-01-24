using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.IO;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSStartChangeNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos,graph);

            subType = SubType.StartChangeNode;
            AddToClassList("ActionNode");
        }
        public void Initialize(Vector2 Pos, string _q_string1, string _Method, GraphView graph)
        {
            base.Initialize(Pos,graph);
            subType = SubType.StartChangeNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
            q_string1 = _q_string1;
            q_string2 = _Method;
        }
        public void Initialize(Vector2 Pos, int id, string name, List<int> cnodes, string _q_string1, string _Method, bool _pause, List<string> extras, GraphView graph, string _Tag)
        {
            Initialize(Pos, _q_string1, _Method,graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            q_bool1 = _pause;
            extraValues = extras; extraValues.Add("");
            Tag = _Tag;
        }
        public override void Draw()
        {
            base.Draw();
            VisualElement customDataContainer = new VisualElement();
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                q_string1 = v.newValue;
            }
           );
            var Dialouges = Directory.GetFiles("Assets\\Dialouge\\Resources\\DialougesData").Where(s => s.EndsWith(".json")); ;

            foreach (string d in Dialouges.ToArray())
            {
                string name = d.Split("\\")[4];
                dropdownobjects.choices.Add(name);
            }
            TextField textField = DSElementUtilities.CreateTextField("StartNode", v => { q_string2 = v.newValue; });
            if(q_string1!=null)
            {
                dropdownobjects.value = q_string1;
                textField.value = q_string2; 
            }
            customDataContainer.Add(dropdownobjects);
            customDataContainer.Add(textField);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
