using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.IO;
    using System.Linq;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSChoiceUnlockNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);

            subType = SubType.ChoiceUnlockNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
        }
        public void Initialize(Vector2 Pos, string _q_string1, string _Method, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.ChoiceUnlockNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
            q_string1 = _q_string1;
            q_string2 = _Method;
        }
        public void Initialize(Vector2 Pos, int id, string name, List<int> cnodes, string _q_string1, string _Method, bool _pause,bool _HasParameter ,List<string> extras, GraphView graph, string _Tag)
        {
            Initialize(Pos, _q_string1, _Method, graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            q_bool1 = _pause;
            q_bool2 = _HasParameter;
            extraValues = extras;
            Tag = _Tag;
        }
        public override void Draw()
        {
            base.Draw();
            VisualElement customDataContainer = new VisualElement();
            if(extraValues.Count==0)
            {
                extraValues.Add("");
                extraValues.Add("");
            }
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
            Toggle toggle = DSElementUtilities.CreateToggle("Locked", v =>
            {
                q_bool1 = v.newValue;
            });
            Toggle Type = DSElementUtilities.CreateToggle("Multinode", v =>
            {
                q_bool2 = v.newValue;
            });
            TextField textField = DSElementUtilities.CreateTextField("Node", v =>     { q_string2 = v.newValue;});
            TextField textField2 = DSElementUtilities.CreateTextField("Value", v => { extraValues[0] = v.newValue; });
            textField2.label = "From [Inclusive]";
            TextField textField3 = DSElementUtilities.CreateTextField("Value", v => { extraValues[1] = v.newValue; });
            textField3.label = "To [Exclusive]";
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if (q_string1 != null)
            {
                dropdownobjects.value = q_string1;
                textField.value = q_string2 == "" ? "Node" : q_string2;
                textField2.value = extraValues[0] == "" ? "Value" : extraValues[0];
                textField3.value = extraValues[1] == "" ? "Value" : extraValues[1];
                toggle.value = q_bool1;
                Type.value = q_bool2;
            }
            textfoldout.Add(Type);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(textField);
            textfoldout.Add(textField2);
            textfoldout.Add(textField3);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
