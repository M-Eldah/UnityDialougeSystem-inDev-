using DSystem.utilities;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DSystem.Windows
{
    
    public class DialougeSystemWindow : EditorWindow
    {
        private DSGraphView dSGraphView;

        [MenuItem("Window/DialougeSystem/Dialouge Graph")]
        public static void Open()
        {
            GetWindow<DialougeSystemWindow>("Dialouge Graph ");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();
        }

        private void AddToolbar()
        {
            Toolbar toolbar = new Toolbar();
            TextField FileNameTextField = DSElementUtilities.CreateTextField("DialougeName");
            Button SaveButton = DSElementUtilities.CreateButton("Save", () => {
                Debug.Log("Saving");
                dSGraphView.save(FileNameTextField.text);
            });
            DropdownField LoadMenu = DSElementUtilities.CreateDropDownMenu("SelectDialouge");
            var Dialouges = Directory.GetFiles("Assets\\Dialouge\\Resources\\DialougesData").Where(s => s.EndsWith(".json")); ;

            foreach(string d in Dialouges.ToArray())
            {
                string name = d.Split("\\")[4];
                LoadMenu.choices.Add(name);
            }

            TextField StartingIndex = DSElementUtilities.CreateTextField("0", evt => dSGraphView.startid = int.Parse(evt.newValue));
            StartingIndex.label = "StartingIndex";
            Button LoadButton = DSElementUtilities.CreateButton("Load",()=> {
                Debug.Log("Loading");
                FileNameTextField.value = LoadMenu.text.Split(".")[0];

                StartingIndex.value = dSGraphView.LoadGraph(LoadMenu.text).ToString() ;
            });
            Button ClearGraph = DSElementUtilities.CreateButton("Clear Graph", () => {
                dSGraphView.ClearGraph();
            });
            toolbar.Add(FileNameTextField);
            toolbar.Add(SaveButton);
            toolbar.Add(LoadMenu);
            toolbar.Add(LoadButton);
            toolbar.Add(StartingIndex);
            toolbar.Add(ClearGraph);
            rootVisualElement.Add(toolbar);
        }

        #region Element Addition

        private void AddGraphView()
        {
            //throw new NotImplementedException();
            DSGraphView graphView = new DSGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            dSGraphView = graphView;
        }

        #endregion Element Addition
    }
}