using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DSystem.Elements
{
    using UnityEditor.UIElements;
    using utilities;
    public class DialougeNode : BaseNode
    {
        public Foldout textfoldout;
       
        protected int offset =0;
        public override void Initialize(Vector2 Pos,GraphView graph)
        {
            base.Initialize(Pos,graph);
            dialougeText = new List<string>();
            choices = new List<string>();
            extraValues = new List<string>();
            mainType = NodeType.DialougeNode;
        }
        public virtual void Initialize(Vector2 Pos, List<string> _dialougeText,List<string> Extra, GraphView graph)
        {
            Initialize(Pos,graph);
            mainType = NodeType.DialougeNode;
            choices = new List<string>();
            dialougeText = _dialougeText;
            extraValues = Extra;
        }
        public virtual void Initialize(Vector2 Pos,List<string> _Choices , List<string> _dialougeText, List<string> Extra, GraphView graph)
        {
            Initialize(Pos, _dialougeText, Extra,graph);
            mainType = NodeType.DialougeNode;
            choices = _Choices;
        }

        public override void Draw()
        {
            //input container
            base.Draw();
            //extension Container
            VisualElement customDataContainer = new VisualElement();
            textfoldout = DSElementUtilities.CreateFoldout("ChoiceState", false);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);


        }
        public virtual void DrawSingle()
        {
            base.Draw();
            //extension Container
            VisualElement customDataContainer = new VisualElement();
            textfoldout = DSElementUtilities.CreateFoldout("Dialouge", false);
            if (dialougeText.Count == 0)
            {
                dialougeText.Add($"Dialouge{dialougeText.Count}");
                extraValues.Add("0"); extraValues.Add("0"); extraValues.Add("False");
                CreateDialougeContainer(dialougeText.Count - 1, dialougeText[dialougeText.Count-1], "0", "0", "False");
            }
            else
            {
                for (int i = 0; i < dialougeText.Count; i++)
                {
                    CreateDialougeContainer(i, dialougeText[i],extraValues[offset + (i *3)], extraValues[offset + (i * 3) + 1], extraValues[offset + (i * 3) + 2]);
                }
            }
            Button addchoice = DSElementUtilities.CreateButton("Add Dialouge", () =>
            {
                dialougeText.Add($"Dialouge{dialougeText.Count}");
                extraValues.Add("0"); extraValues.Add("0"); extraValues.Add("False");
                CreateDialougeContainer(dialougeText.Count-1, dialougeText[dialougeText.Count - 1], "0", "0", "False");
            }
            );
            customDataContainer.Add(addchoice);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
        }
        private void CreateDialougeContainer(int i, string text,string extra,string extra2)
        {
            TextField textField = DSElementUtilities.CreateTextArea(text, evt => { dialougeText[i] = evt.newValue; });
            textField.AddToClassList("SpeachdialougeText");
            Foldout Extra = DSElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = DSElementUtilities.CreateTextField(extra, evt => { extraValues[(i * 3)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = DSElementUtilities.CreateTextField(extra2, evt => { extraValues[(i * 3) + 1] = evt.newValue; });
            id.label = "FaceID";
           
            Extra.Add(Actor);
            Extra.Add(id);
            textfoldout.Add(textField);
            textfoldout.Add(Extra);
        }
        private void CreateDialougeContainer(int i, string text, string extra, string extra2, string extra3)
        {
            VisualElement cont = new VisualElement();
            TextField textField = DSElementUtilities.CreateTextArea(text, evt => { int index = Getindex(evt.previousValue);dialougeText[index] = evt.newValue;  });
            textField.AddToClassList("SpeachdialougeText");
            Foldout Extra = DSElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = DSElementUtilities.CreateTextField(extra, evt => { int index = Getindex(textField.value); extraValues[offset+(index * 3)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = DSElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(textField.value); extraValues[offset + (index * 3) + 1] = evt.newValue; });
            id.label = "FaceID";
            Toggle toggle = DSElementUtilities.CreateToggle("Skip", evt => { int index = Getindex(textField.value); extraValues[offset + (index * 3) + 2] = evt.newValue.ToString(); });
            toggle.value = bool.Parse(extra3);
            Button Delte = DSElementUtilities.CreateButton("Remove Dialouge", () =>
            {
                int index = Getindex(textField.value);
                Debug.Log(index);

                dialougeText.RemoveAt(index);
                extraValues.RemoveAt(offset + (index * 3) + 2);
                extraValues.RemoveAt(offset + (index * 3) + 1);
                extraValues.RemoveAt(offset + (index * 3));                
                textfoldout.Remove(cont);
            });
            Extra.Add(Actor);
            Extra.Add(id);
            Extra.Add(toggle);
            cont.Add(textField);
            cont.Add(Extra);
            cont.Add(Delte);
            textfoldout.Add(cont);

        }
        private int Getindex(string text)
        {
            int index = dialougeText.FindIndex(x => x == text);
            return index;
        }
    }
}