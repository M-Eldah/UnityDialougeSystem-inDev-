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
            base.DrawSingle(); 
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
            evt =>
            {
                q_string2 = evt.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v =>
            {
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
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                dropdownobjects.choices.Add(obj.name);
            }

            textfoldout.Insert(0,ValueType);
            textfoldout.Insert(1, Direction);
            textfoldout.Insert(2, dropdownobjects);
            textfoldout.Insert(3, dropdownmethods);
            if(q_string1!=null)
            {
                ValueType.value = q_bool1;
                Direction.value = q_bool2;
                dropdownobjects.value = q_string1;
                dropdownmethods.value = q_string2;
            }
            RefreshExpandedState();

        }


    }
}

