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
    public class DSValueDirectionNode : DialougeNode
    {
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.ValueDirectionNode;
            AddToClassList("MultiNode");

        }
        public override void Initialize(Vector2 Pos, List<string> _Choices, List<string> _Text, List<string> extra, GraphView graph)
        {
            base.Initialize(Pos, _Text, extra, graph);
            subType = SubType.ValueDirectionNode;
            AddToClassList("MultiNode");
            choices = _Choices;
            dialougeText = _Text;
            SetPosition(new Rect(Pos, Vector2.zero));
            RefreshExpandedState();
        }
        public void Initialize(Vector2 Pos, List<string> _Choices, List<string> _Text, int id, string name, List<int> cnodes, List<string> extra, GraphView graph, string _Tag)
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
            base.Draw();
            if (extraValues.Count == 0)
            {
                extraValues.Add("False"); extraValues.Add("False");
                extraValues.Add("NA"); extraValues.Add("NA");
            }
            Toggle ValueType = DSElementUtilities.CreateToggle("Property");
            ValueType.tooltip = "Type of value you want to change";
            ValueType.RegisterValueChangedCallback(evt =>
            {
                extraValues[0] = evt.newValue.ToString();
                ValueType.label = bool.Parse(extraValues[0]) ? "Field" : "Property";
            }
            );
            Toggle Direction = DSElementUtilities.CreateToggle("Lower");
            Direction.tooltip = "Type of value you want to change";
            Direction.RegisterValueChangedCallback(evt =>
            {
                extraValues[1] = evt.newValue.ToString();
                Direction.label = bool.Parse(extraValues[1]) ? "Greater" : "Lower";
            }
            );
            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Properties",
            evt => {
                extraValues[3] = evt.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                extraValues[2] = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropdownmethods.choices.Clear();
                    if (!bool.Parse(extraValues[0]))
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
            Button addchoice = DSElementUtilities.CreateButton("Add Directions", () =>
            {
                choices.Add("Value");
                Port Choice = CreateChoice(choices.Count - 1);
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

            //Port Container
            if (choices.Count == 0)
            {
                choices.Add("Value");
                Port Choice = CreateChoice(choices.Count - 1);
                outputContainer.Add(Choice);
                RefreshExpandedState();
            }
            else
            {
                ValueType.value = bool.Parse(extraValues[0]);
                Direction.value = bool.Parse(extraValues[1]);
                dropdownobjects.value = extraValues[2];
                dropdownmethods.value = extraValues[3];
                for (int i = 0; i < choices.Count; i++)
                {
                    Port Choice = CreateChoice(i);
                    outputContainer.Add(Choice);
                }
            }
        }
        #region Choice Element Creation
        private Port CreateChoice(int id)
        {
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            Choice.portName = $"Output";
            TextField Value = DSElementUtilities.CreateTextField(choices[id], evt => {
                int index = Getindex(evt.previousValue);
                choices[index] = evt.newValue.ToString();
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
                int index = Getindex(Value.value);
                choices.RemoveAt(id);
                GraphView.RemoveElement(Choice);

            });

            DeleteChoice.AddToClassList("DeleteButton");
            Choice.Add(DeleteChoice);
            Choice.Add(Value);
            return Choice;
        }
        private int Getindex(string text)
        {
            return choices.FindIndex(x => x == text);
        }
        #endregion
    }

}
