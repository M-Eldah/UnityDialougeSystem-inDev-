using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
namespace DSystem.Elements
{
    using UnityEngine.UIElements;
    using utilities;
    public class DSGroup : Group
    {
        public string GroupName;
        public List<int> ContainedNodes;
        public Vector2 pos;
        public DSGroup()
        {
            ContainedNodes = new List<int>();
        }
        public void saveGroup()
        {
            ContainedNodes.Clear();
            GroupName = title;
            Debug.Log(GroupName);
            foreach(VisualElement element in containedElements)
            {
                if (element is BaseNode Dnode)
                {
                    ContainedNodes.Add(Dnode.Id);
                }
            }
        }
        public virtual void Initialize(Vector2 Pos)
        {
            pos = Pos;
            SetPosition(new Rect(Pos, Vector2.zero));
        }
        public virtual void Draw()
        {
           
        }
    }

}
