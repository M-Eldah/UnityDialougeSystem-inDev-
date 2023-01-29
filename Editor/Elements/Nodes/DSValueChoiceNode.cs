using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System;
    using System.Reflection;
    using utilities;
    public class DSValueChoiceNode : DialougeNode
    {
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.ValueChoiceNode;
            AddToClassList("MultiNode");

        }
        public override void Initialize(Vector2 Pos, List<string> _Choices, List<string> _Text, List<string> extra, GraphView graph)
        {
            base.Initialize(Pos, _Text, extra, graph);
            subType = SubType.ValueChoiceNode;
            AddToClassList("MultiNode");
            choices = _Choices;
            dialougeText = _Text;
            SetPosition(new Rect(Pos, Vector2.zero));
            RefreshExpandedState();
        }
        public void Initialize(Vector2 Pos, List<string> _Choices, List<string> _Text, int id, string name, List<int> cnodes, List<string> extra, GraphView graph,string _Tag)
        {
            base.Initialize(Pos, _Choices, _Text, extra, graph);
            SetPosition(new Rect(Pos, Vector2.zero));
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            Tag = _Tag;

        }
        public override void Draw()
        {
            //Main Container
            base.Draw(true,"Value");
            Toggle ValueType = DSElementUtilities.CreateToggle("Property");
            ValueType.tooltip = "Type of value you want to change";
            ValueType.RegisterValueChangedCallback(evt =>
            {
                q_bool1 = evt.newValue;
                ValueType.label = q_bool1 ? "Field" : "Property";
            }
            );
            Toggle Direction = DSElementUtilities.CreateToggle("Lower");
            Direction.tooltip = "Type of value you want to change";
            Direction.RegisterValueChangedCallback(evt =>
            {
                q_bool2 = evt.newValue;
                Direction.label = q_bool2 ? "Greater" : "Lower";
            }
            );
            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Properties",
            evt => { q_string2 = evt.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                q_string1 = v.newValue;
                    GameObject gameObject = GameObject.Find(v.newValue);
                 if (gameObject != null)
                 {
                     dropdownmethods.choices.Clear();
                     if (!q_bool1)
                     {
                         List<PropertyInfo> methodz = UtilityFunctions.GetProperties(gameObject);
                         foreach (PropertyInfo method in methodz)
                         {
                             dropdownmethods.choices.Add(method.Name);
                         }
                     }
                     else
                     {
                         List<FieldInfo> methodz = UtilityFunctions.GetFields(gameObject);
                         foreach (FieldInfo method in methodz)
                         {
                             dropdownmethods.choices.Add(method.Name);
                         }
                     }

                 }
            }
            );
            textfoldout.Add(ValueType);
            textfoldout.Add(Direction);
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(dropdownmethods);
            Button addchoice = DSElementUtilities.CreateButton("Add Choice", () =>
            {
                choices.Add($"New Choice{choices.Count}");
                extraValues.Add("Value");
                VisualElement Choice = CreateChoice(choices.Count - 1);
                outputContainer.Add(Choice);
                RefreshExpandedState();
            }
            );
            mainContainer.Insert(1, addchoice);

            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                dropdownobjects.choices.Add(obj.name);
            }

            if (choices.Count == 0)
            {
                choices.Add($"New Choice{choices.Count}");
                extraValues.Add("Value");
                VisualElement Choice = CreateChoice(choices.Count - 1);
                outputContainer.Add(Choice);
                RefreshExpandedState();
            }
            else
            {
                ValueType.value = q_bool1;
                Direction.value = q_bool2;
                dropdownobjects.value = q_string1;
                dropdownmethods.value = q_string2;
                for (int i = 0; i < choices.Count; i++)
                {
                    VisualElement Choice = CreateChoice(i);
                    outputContainer.Add(Choice);
                }
            }
            ToggleCollapse(); ToggleCollapse();
        }
        #region Choice Element Creation
        private VisualElement CreateChoice(int id)
        {
            VisualElement container = new VisualElement();
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            Choice.portName = $"Output";
            VisualElement container2 = new VisualElement();
            container.AddToClassList("multiContainer"); 
            container2.AddToClassList("secondaryContainer");
            TextField choiceTextfield = DSElementUtilities.CreateTextField(choices[id], evt =>
            {
                int indeX = Getindex(evt.previousValue);
                choices[indeX] = evt.newValue;
            });
            TextField ElementToggle = DSElementUtilities.CreateTextField(extraValues[id], evt => {
                int indeX = Getindex(choiceTextfield.value);
                extraValues[indeX] =evt.newValue.ToString();
            });
            Button DeleteChoice = DSElementUtilities.CreateButton("X", () => {
                if (choices.Count == 1)
                {
                    return;
                }
                if (Choice.connected)
                {
                    GraphView.DeleteElements(Choice.connections);
                }
                int indeX = Getindex(choiceTextfield.value);
                choices.RemoveAt(indeX);
                extraValues.RemoveAt(indeX);
                outputContainer.Remove(container);
            });
            output.Add(Choice); container2.Add(Choice);
            container2.Add(ElementToggle);
        
            container.Add(container2);
            container.Add(choiceTextfield);
            container.Add(DeleteChoice);
            return container;
        }
        private int Getindex(string text)
        {
            return choices.FindIndex(x => x == text);
        }
        #endregion
    }

}