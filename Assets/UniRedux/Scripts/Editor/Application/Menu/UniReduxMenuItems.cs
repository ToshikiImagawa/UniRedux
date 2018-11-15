using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Editor.Menu
{
    public static class UniReduxMenuItems
    {
        [MenuItem("Assets/Create/UniRedux/Create UniReduxApplication", false)]
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

            var absolutePath = EditorUtility.SaveFilePanel(
                "Choose name for " + friendlyName,
                folderPath,
                defaultFileName + ".cs",
                "cs");

            if (absolutePath == "")
            {
                // Dialog was cancelled
                return;
            }

            if (!absolutePath.ToLower().EndsWith(".cs"))
            {
                absolutePath += ".cs";
            }

            var className = Path.GetFileNameWithoutExtension(absolutePath);
            File.WriteAllText(absolutePath, templateStr.Replace("CLASS_NAME", className));

            AssetDatabase.Refresh();

            absolutePath = Path.GetFullPath(absolutePath);

            var assetFolderFullPath = Path.GetFullPath(UnityEngine.Application.dataPath);

            var assetPath = "Assets";
            if (absolutePath.Length == assetFolderFullPath.Length)
            {
                if (absolutePath != assetFolderFullPath) throw Assert.CreateException();
            }
            else
            {
                assetPath = absolutePath.Remove(0, assetFolderFullPath.Length + 1).Replace("\\", "/");
                assetPath = "Assets/" + assetPath;
            }

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
        }
    }
}