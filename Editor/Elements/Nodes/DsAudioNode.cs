using UnityEngine;
using System.Collections.Generic;
namespace DSystem.Elements
{
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEditor.UIElements;
    using UnityEngine.UIElements;
    using utilities;
    public class DSAudioNode : UtilityNode
    {

        public override void Initialize(Vector2 Pos, GraphView graph)
        {
            base.Initialize(Pos, graph);

            subType = SubType.AudioNode;
            AddToClassList("ActionNode");
        }
        public void Initialize(Vector2 Pos, string _q_string1, string _Method, GraphView graph)
        {
            base.Initialize(Pos, graph);
            subType = SubType.AudioNode;
            AddToClassList("ActionNode");
            choices.Add("Dialouge");
            q_string1 = _q_string1;
        }
        public void Initialize(Vector2 Pos, int id, string name, List<int> cnodes, string _q_string1, string _Method, bool _pause, List<string> extras, GraphView graph, string _Tag)
        {
            Initialize(Pos, _q_string1, _Method, graph);
            connections = cnodes;
            Id = id;
            title = nodeName = name;
            q_bool1 = _pause;
            extraValues = extras; extraValues.Add("");
            Tag = _Tag;
        }
        public override void Draw()
        {
            base.Draw();
            VisualElement customDataContainer = new VisualElement();

            ObjectField objectField = DSElementUtilities.Objectfield("Audio Clip:",evt=>
            {
               q_string1 =evt.newValue.name;
            });
            Toggle pauseToggle = DSElementUtilities.CreateToggle("Pause", evt => q_bool1 =evt.newValue);
            objectField.objectType = typeof(AudioClip);  
            if (q_string1 != null)
            {
                objectField.value = Resources.Load(q_string1);
                pauseToggle.value = q_bool1;
            }
            customDataContainer.Add(objectField);
            customDataContainer.Add(pauseToggle);
            extensionContainer.Add(customDataContainer);
            RefreshExpandedState();
        }

    }
}
