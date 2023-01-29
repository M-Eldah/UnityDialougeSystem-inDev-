using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using System;
    using UnityEngine.Accessibility;
    using utilities;
    public class DSMultiNode : DialougeNode
    {
        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.MultiNode;
            AddToClassList("MultiNode");

        }
        public override void Initialize(Vector2 Pos, List<string> _Choices, List<string> _Text, List<string> extra, GraphView graph)
        {
            base.Initialize(Pos, _Text, extra, graph);
            subType = SubType.MultiNode;
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
            Button addchoice = DSElementUtilities.CreateButton("Add Choice", () =>
            {
                choices.Add($"New Choice{choices.Count}");
                extraValues.Add("False");
                VisualElement Choice = CreateChoice(choices.Count - 1);
                outputContainer.Add(Choice);
                RefreshExpandedState();
            }
            );
            mainContainer.Insert(1, addchoice);

            //Port Container
            if (choices.Count == 0)
            {
                choices.Add($"New Choice{choices.Count}");
                extraValues.Add("False");
                VisualElement Choice = CreateChoice(choices.Count - 1);
                outputContainer.Add(Choice);
                RefreshExpandedState();
            }
            else
            {
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
            container.AddToClassList("multiContainer");
            Port Choice = this.CreatePort("", Orientation.Horizontal, Direction.Output, Port.Capacity.Single);
            output.Add( Choice );
            Choice.portName = $"Output";
            TextField choiceTextfield = DSElementUtilities.CreateTextField(choices[id], evt => 
            {
                int indeX = Getindex(evt.previousValue);
                choices[indeX] = evt.newValue; 
            });
            Toggle ElementToggle = DSElementUtilities.CreateToggle("locked", evt => {
                int indeX = Getindex(choiceTextfield.value);
                extraValues[indeX] = (string)Convert.ChangeType(evt.newValue, typeof(string));
                });
            
                ElementToggle.value = (Boolean)Convert.ChangeType(extraValues[id], typeof(Boolean));
                Button DeleteChoice = DSElementUtilities.CreateButton("X", ()=> { 
                if(choices.Count==1)
                {
                    return;
                }
                if(Choice.connected)
                {
                    GraphView.DeleteElements(Choice.connections);
                }
                int indeX = Getindex(choiceTextfield.value);

                choices.RemoveAt(indeX);
                extraValues.RemoveAt(indeX);output.Remove(Choice);
                outputContainer.Remove(container);
            });
            VisualElement container2 = new VisualElement();
            container2.AddToClassList("secondaryContainer");
            DeleteChoice.AddToClassList("DeleteButton");
            choiceTextfield.AddToClassList("ChoiceText");
            container2.Add(ElementToggle);
            container2.Add(Choice);
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