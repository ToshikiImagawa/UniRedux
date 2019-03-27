using System.IO;
using System.Linq;
using UniRedux.EventSystems;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Editor.Menu.Application
{
    public static class UniReduxApplicationMenuItems
    {
        [MenuItem("Assets/Create/UniRedux/Application/Create UniReduxApplication", false)]
        public static void CreateReduxApplications()
        {
            const string friendlyName = "UniReduxApplication";
            var selectObject = Selection.objects.FirstOrDefault();
            var folderPath = selectObject != null ? AssetDatabase.GetAssetPath(selectObject) : string.Empty;
            const string defaultFileName = "UntitledApplication";
            const string templateStr = "using UnityEngine;"
                                       + "\nusing UniRedux;"
                                       + "\n"
                                       + "\n[CreateAssetMenu(fileName = \"UniReduxApplication\", menuName = \"UniRedux/Applications/CLASS_NAME\")]"
                                       + "\npublic class CLASS_NAME : UniReduxApplication<TemplateState>"
                                       + "\n{"
                                       + "\n    protected override IStore<TemplateState> CrateStore { get; } // todo: CrateStore"
                                       + "\n}";

            UniReduxMenuItems.CreateTemplateFile(friendlyName, folderPath, defaultFileName, templateStr);
        }
        
#if UNITY_EDITOR
        [MenuItem("Assets/Create/UniRedux/Application/ProjectEventSystem", priority = 30)]
        public static void CreatePrefab()
        {
            var path = UniReduxEditorUtility.GetSelectDirectoryPath;

            if (string.IsNullOrEmpty(path) || Path.GetFileName(path) != "Resources") throw Assert.CreateException();

            var gameObject =
                EditorUtility.CreateGameObjectWithHideFlags("ProjectEventSystem",
                    HideFlags.HideInHierarchy);
            gameObject.AddComponent<ProjectEventSystem>();
            PrefabUtility.CreatePrefab($"{UniReduxEditorUtility.ConvertFullAbsolutePathToAssetPath(path)}/ProjectEventSystem.prefab", gameObject);
            Object.DestroyImmediate(gameObject);
        }
#endif
    }
}