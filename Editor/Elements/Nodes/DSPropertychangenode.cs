using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.Reflection;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSPropertychangenode : UtilityNode
{

        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);

            subType = SubType.Propertychangenode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
        }
        public void Initialize(Vector2 Pos, string _q_string1, string _Method, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.Propertychangenode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
            q_string1 = _q_string1;
            q_string2 = _Method;

        }
        public void Initialize(Vector2 Pos, int id, string name, List<int> cnodes, string _q_string1, string _Method, bool _pause, List<string> extras, GraphView graph, string _Tag)
        {
            Initialize(Pos, _q_string1, _Method, graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            q_bool1 = _pause;
            extraValues = extras;
            Tag = _Tag;
        }
        public override void Draw()
        {
            base.Draw();
            VisualElement customDataContainer = new VisualElement();


            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Properties", v =>
            {
                q_string2 = v.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                q_string1 = v.newValue;
                GameObject q_string1ect = GameObject.Find(v.newValue);
                if (q_string1ect != null)
                {
                    dropdownmethods.choices.Clear();
                    if(!q_bool1)
                    {
                        List<PropertyInfo> methodz = UtilityFunctions.GetProperties(q_string1ect);
                        foreach (PropertyInfo method in methodz)
                        {
                            dropdownmethods.choices.Add(method.Name);
                        }
                    }
                    else
                    {
                        List<FieldInfo> methodz = UtilityFunctions.GetFields(q_string1ect);
                        foreach (FieldInfo method in methodz)
                        {
                            dropdownmethods.choices.Add(method.Name);
                        }
                    }
                }
            }
            );
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                dropdownobjects.choices.Add(obj.name);
            }
            Toggle toggle = DSElementUtilities.CreateToggle("Property");
            toggle.tooltip = "Type of value you want to change";
            toggle.RegisterValueChangedCallback(evt =>
            {
                q_bool1 = evt.newValue;
                toggle.label = q_bool1 ? "Field" : "Property";
            }
            );
            Toggle modify = DSElementUtilities.CreateToggle("Modify");
            modify.tooltip = "should it modify the property/field by value, or just set it equal to to it";
            modify.RegisterValueChangedCallback(evt =>
            {
                q_bool2 = evt.newValue;
                modify.label = q_bool2 ? "Modify" : "Set";
            }
            );
            TextField textField = DSElementUtilities.CreateTextField("Value", v => { extraValues[0] = v.newValue;});
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if (q_string1 != null)
            {
                dropdownobjects.value = q_string1;
                dropdownmethods.value = q_string2;
                textField.value = extraValues[0] == "" ? "Parameters" : extraValues[0];
                toggle.value = q_bool1;
                modify.value = q_bool2;
            }
            textfoldout.Add(toggle);
            textfoldout.Add(modify);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(dropdownmethods);
            textfoldout.Add(textField);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
