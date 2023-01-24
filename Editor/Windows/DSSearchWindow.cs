using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DSystem.Windows
{
    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView graphView;

        public void Intialiaze(DSGraphView dSGraphView)
        {
            graphView = dSGraphView;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Dialouge Nodes"),1),
                new SearchTreeEntry(new GUIContent("Single Node"))
                {
                     level=2,userData=SubType.SingleNode
                },
                new SearchTreeEntry(new GUIContent("MultiNode"))
                {
                     level=2,userData=SubType.MultiNode
                },
                new SearchTreeGroupEntry(new GUIContent("Function Node"),1),
                new SearchTreeEntry(new GUIContent("Action Node"))
                {
                     level=2,userData=SubType.ActionNode
                },
                new SearchTreeGroupEntry(new GUIContent("DialougeGroup"),1),
                new SearchTreeEntry(new GUIContent("SingleGroup"))
                {
                     level=2,userData=new Group()
                },
            };
            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            Vector2 localmousepos = graphView.GetLocalMousePosition(context.screenMousePosition, true);
            switch (SearchTreeEntry.userData)
            {
                case SubType.SingleNode:
                    graphView.CreateNode(NodeType.DialougeNode, SubType.SingleNode, localmousepos);
                    return true;
                case SubType.MultiNode:
                    graphView.CreateNode(NodeType.DialougeNode, SubType.MultiNode, localmousepos);
                    return true;
                case SubType.ActionNode:
                    graphView.CreateNode(NodeType.UtilityNode, SubType.ActionNode, localmousepos);
                    return true;
                case Group:
                    Group group = graphView.Creategroup("Dialouge Group", context.screenMousePosition);
                    graphView.AddElement(group);
                    return true;
                default:
                    return false;
            }
        }
    }
}