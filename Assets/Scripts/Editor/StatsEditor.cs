using RPG.Core;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace RPG.Editor
{
    public class StatsEditor : OdinMenuEditorWindow
    {
        [MenuItem("RPG/Stats Editor")]
        static void OpenWindow()
        {
            GetWindow<StatsEditor>().Show();
        }
        
        private CreateNewProgression createNewProgression;

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (createNewProgression != null)
            {
                DestroyImmediate(createNewProgression.progression);
            }
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            createNewProgression = new CreateNewProgression();
            tree.Add("Create New Progression", createNewProgression);
            tree.AddAllAssetsAtPath("Stats", "Assets/Game/Core", typeof(Progression));

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            //Gets reference to currently selected item
            OdinMenuTreeSelection selected = this.MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();
                
                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    Progression asset = selected.SelectedValue as Progression;
                    string path = AssetDatabase.GetAssetPath(asset);
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        public class CreateNewProgression
        {
            public CreateNewProgression()
            {
                progression = ScriptableObject.CreateInstance<Progression>();
                progression.name = "New Progression";
            }

            public string assetName;
            
            [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
            public Progression progression;

            [Button("Create New Progression Data")]
            void CreateNewData()
            {
                AssetDatabase.CreateAsset(progression, "Assets/Game/Core/" + assetName + ".asset");
                AssetDatabase.SaveAssets();

                //Create new instance of the SO
                progression = ScriptableObject.CreateInstance<Progression>();
                progression.name = "New Progression";
            }
        }
    }
}