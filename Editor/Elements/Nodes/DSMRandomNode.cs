using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
namespace DSystem.Elements
{
    using System.Reflection;
    using UnityEngine.UIElements;
    using utilities;
    public class DSMRandomNode : DialougeNode
    {
        public Port Choice;
        public override void Initialize(Vector2 Pos, GraphView graph)
        {

            base.Initialize(Pos, graph);
            subType = SubType.MRandomNode;
            AddToClassList("SingleNode");
            choices.Add("Dialouge");
        }
        public override void Initialize(Vector2 Pos, List<string> _Text, List<string> extra, GraphView graph)
        {

            base.Initialize(Pos, _Text, extra, graph);
            subType = SubType.MRandomNode;
            AddToClassList("SingleNode");
            choices.Add("Dialouge");
        }
        public void Initialize(Vector2 Pos, List<string> _Text, int id, string name, List<int> cnodes, List<string> extra, GraphView graph, string _Tag)
        {
           
            Initialize(Pos, _Text, extra, graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            Tag = _Tag;
        }
        public override void DrawSingle()
        {
            offset = 4;
            base.DrawSingle();
            Choice = this.CreatePort("Output");
            Choice.portName = $"{Id}Output";
            outputContainer.Add(Choice);
            if (extraValues.Count == 3)
            {
                extraValues.Insert(0,"False"); extraValues.Insert(1,"False");
                extraValues.Insert(2,"NA"); extraValues.Insert(3,"NA");
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
            evt =>
            {
                extraValues[3] = evt.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
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
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                dropdownobjects.choices.Add(obj.name);
            }

            textfoldout.Insert(0,ValueType);
            textfoldout.Insert(1, Direction);
            textfoldout.Insert(2, dropdownobjects);
            textfoldout.Insert(3, dropdownmethods);
            if(extraValues.Count>3)
            {
                ValueType.value = bool.Parse(extraValues[0]);
                Direction.value = bool.Parse(extraValues[1]);
                dropdownobjects.value = extraValues[2];
                dropdownmethods.value = extraValues[3];
            }
            RefreshExpandedState();

        }


    }
}

