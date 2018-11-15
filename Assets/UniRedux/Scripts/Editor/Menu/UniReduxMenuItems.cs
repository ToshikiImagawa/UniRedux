using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Editor.Menu
{
    public static class UniReduxMenuItems
    {

        public static void CreateTemplateFile(string friendlyName, string folderPath, string defaultFileName,
            string templateStr)
        {
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