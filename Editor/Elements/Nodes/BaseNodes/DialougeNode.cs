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

        public void Draw(bool extension,string drawName)
        {
            //input container
            base.Draw();
            //extension Container
            if(extension) 
            {
                VisualElement customDataContainer = new VisualElement();
                textfoldout = DSElementUtilities.CreateFoldout(drawName, false);
                customDataContainer.Add(textfoldout);
                extensionContainer.Add(customDataContainer);
            }
        }
        public virtual void DrawSingle()
        {
            base.Draw();
            Port Choice = this.CreatePort("Output");
            output.Add(Choice);
            Choice.portName = $"{Id}Output";
            outputContainer.Add(Choice);
            //extension Container
            VisualElement customDataContainer = new VisualElement();
            textfoldout = DSElementUtilities.CreateFoldout("Dialouge", false);
            if (dialougeText.Count == 0)
            {
                dialougeText.Add($"Dialouge{dialougeText.Count}");
                extraValues.Add("0"); extraValues.Add("0"); extraValues.Add("False");
                CreateDialougeContainer(dialougeText[dialougeText.Count-1], "0", "0", "False");
            }
            else
            {
                for (int i = 0; i < dialougeText.Count; i++)
                {
                    CreateDialougeContainer(dialougeText[i],extraValues[(i *3)], extraValues[ (i * 3) + 1], extraValues[(i * 3) + 2]);
                }
            }
            Button addchoice = DSElementUtilities.CreateButton("Add Dialouge", () =>
            {
                dialougeText.Add($"Dialouge{dialougeText.Count}");
                extraValues.Add(extraValues[extraValues.Count-3]); extraValues.Add(extraValues[extraValues.Count - 3]); extraValues.Add("False");
                CreateDialougeContainer(dialougeText[dialougeText.Count - 1], extraValues[extraValues.Count - 3], extraValues[extraValues.Count - 2], "False");
            }
            );
            customDataContainer.Add(addchoice);
            customDataContainer.Add(textfoldout);
            extensionContainer.Add(customDataContainer);
        }
        private void CreateDialougeContainer(string text, string extra, string extra2, string extra3)
        {
            VisualElement cont = new VisualElement();
            TextField textField = DSElementUtilities.CreateTextArea(text, evt => { int index = Getindex(evt.previousValue);dialougeText[index] = evt.newValue;  });
            textField.AddToClassList("SpeachdialougeText");
            Foldout Extra = DSElementUtilities.CreateFoldout("Extra", true);
            TextField Actor = DSElementUtilities.CreateTextField(extra, evt => { int index = Getindex(textField.value); extraValues[(index * 3)] = evt.newValue; });
            Actor.label = "Actor";
            TextField id = DSElementUtilities.CreateTextField(extra2, evt => { int index = Getindex(textField.value); extraValues[(index * 3) + 1] = evt.newValue; });
            id.label = "FaceID";
            Toggle toggle = DSElementUtilities.CreateToggle("Skip", evt => { int index = Getindex(textField.value); extraValues[(index * 3) + 2] = evt.newValue.ToString(); });
            toggle.value = bool.Parse(extra3);
            Button Delte = DSElementUtilities.CreateButton("Remove Dialouge", () =>
            {
                int index = Getindex(textField.value);
                Debug.Log(index);

                dialougeText.RemoveAt(index);
                extraValues.RemoveAt((index * 3) + 2);
                extraValues.RemoveAt((index * 3) + 1);
                extraValues.RemoveAt((index * 3));                
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