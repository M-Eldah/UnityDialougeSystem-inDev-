using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.Reflection;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;
    using utilities;
    public class DSActionNode : UtilityNode
    {
        
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos,graph);
            subType = SubType.ActionNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
        }
        public void Initialize(Vector2 Pos, string _q_string1,string _Method, GraphView graph)
        {
            base.Initialize(Pos,graph);
            subType = SubType.ActionNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
            q_string1 = _q_string1;
            q_string2 = _Method;

        }
        public void Initialize(Vector2 Pos, int id, string name, List<int> cnodes, string _q_string1, string _Method,bool _pause,List<string> extras, GraphView graph, string _Tag)
        {
            Initialize(Pos, _q_string1,_Method,graph);
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


            DropdownField dropdownmethods = DSElementUtilities.CreateDropDownMenu("Methods", v =>
            {
                q_string2 = v.newValue;
            });
            DropdownField dropdownobjects = DSElementUtilities.CreateDropDownMenu("Objects", v => {
                q_string1 = v.newValue;
                GameObject gameObject = GameObject.Find(v.newValue);
                if (gameObject != null)
                {
                    dropdownmethods.choices.Clear();
                    List<MethodInfo> methodz = UtilityFunctions.GetMethods(gameObject);
                    foreach (MethodInfo method in methodz)
                    {
                        dropdownmethods.choices.Add(method.Name);
                    }
                }
            }
            );
            var objects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in objects)
            {
                dropdownobjects.choices.Add(obj.name);
            }
            Toggle toggle = DSElementUtilities.CreateToggle("Pause Here", v =>
            {
                q_bool1 = v.newValue;
            });
            TextField textField = DSElementUtilities.CreateTextField("Parameters", v => { extraValues[0] = v.newValue;q_bool2 = true; }) ;
            Foldout textfoldout = DSElementUtilities.CreateFoldout("Data", false);
            if(q_string1 !=null)
            {
                dropdownobjects.value=q_string1;
                dropdownmethods.value = q_string2;
                textField.value = extraValues[0]==""? "Parameters":extraValues[0];
                q_bool2 = extraValues[0] == "" ? false : true;
                toggle.value = q_bool1;
            }
        
            textfoldout.Add(dropdownobjects);
            textfoldout.Add(dropdownmethods);
            textfoldout.Add(textField);
            textfoldout.Add(toggle);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }
        
    }
}