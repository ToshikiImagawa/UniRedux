using System;
using System.IO;
using System.Linq;
using UniRedux.Provider;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace UniRedux.Editor.Menu.Provider
{
    public static class UniReduxProviderMenuItems
    {
        [MenuItem("Assets/Create/UniRedux/Provider/Create MonoContainerInstaller", false)]
        public static void CreateMonoContainerInstaller()
        {
            const string friendlyName = "UniReduxProvider";
            var selectObject = Selection.objects.FirstOrDefault();
            var folderPath = selectObject != null ? AssetDatabase.GetAssetPath(selectObject) : string.Empty;
            const string defaultFileName = "UntitledMonoContainerInstaller";
            const string templateStr = "using UnityEngine;"
                                       + "\nusing UniRedux;"
                                       + "\n"
                                       + "\n[CreateAssetMenu(fileName = \"CLASS_NAME\", menuName = \"UniRedux/ContainerInstallers/CLASS_NAME\")]"
                                       + "\npublic class CLASS_NAME : MonoContainerInstaller"
                                       + "\n{"
                                       + "\n    public override void InstallBindings(IContainerBundle containerBundle)"
                                       + "\n    {"
                                       + "\n        // containerBundle.SetUniReduxContainer(containerName,container);"
                                       + "\n    }"
                                       + "\n}";

            UniReduxMenuItems.CreateTemplateFile(friendlyName, folderPath, defaultFileName, templateStr);
        }

        [MenuItem("Assets/Create/UniRedux/Provider/Create ScriptableObjectContainerInstaller", false)]
        public static void CreateScriptableObjectContainerInstaller()
        {
            const string friendlyName = "UniReduxProvider";
            var selectObject = Selection.objects.FirstOrDefault();
            var folderPath = selectObject != null ? AssetDatabase.GetAssetPath(selectObject) : string.Empty;
            const string defaultFileName = "UntitledScriptableObjectContainerInstaller";
            const string templateStr = "using UnityEngine;"
                                       + "\nusing UniRedux.Provider;"
                                       + "\n"
                                       + "\n[CreateAssetMenu(fileName = \"CLASS_NAME\", menuName = \"UniRedux/ContainerInstallers/CLASS_NAME\")]"
                                       + "\npublic class CLASS_NAME : ScriptableObjectContainerInstaller"
                                       + "\n{"
                                       + "\n    public override void InstallBindings(IContainerBundle containerBundle)"
                                       + "\n    {"
                                       + "\n        // containerBundle.SetUniReduxContainer(containerName,container);"
                                       + "\n    }"
                                       + "\n}";

            UniReduxMenuItems.CreateTemplateFile(friendlyName, folderPath, defaultFileName, templateStr);
        }

        [MenuItem("Assets/Create/UniRedux/Provider/ProjectContainerBundle", false, priority = 30)]
        public static void CreatePrefab()
        {
            var path = UniReduxEditorUtility.GetSelectDirectoryPath;

            if (string.IsNullOrEmpty(path) || Path.GetFileName(path) != "Resources") throw Assert.CreateException();

            var gameObject =
                EditorUtility.CreateGameObjectWithHideFlags("ProjectContainerBundle",
                    HideFlags.HideInHierarchy);
            gameObject.AddComponent<ProjectContainerBundle>();
            PrefabUtility.CreatePrefab(
                $"{UniReduxEditorUtility.ConvertFullAbsolutePathToAssetPath(path)}/ProjectContainerBundle.prefab",
                gameObject);
            Object.DestroyImmediate(gameObject);
        }

        [MenuItem("GameObject/UniRedux/Provider/SceneContainerBundle", false, priority = 10)]
        public static void CreateSceneEventSystem()
        {
            var gameObject = SceneManager.GetActiveScene().GetRootGameObjects()
                .FirstOrDefault(obj => obj.gameObject.name == "SceneContainerBundle");
            if (gameObject != null) throw Assert.CreateException("SceneContainerBundle exists on scene.");
            gameObject = new GameObject("SceneContainerBundle");
            gameObject.AddComponent<SceneContainerBundle>();
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localRotation = Quaternion.identity;
        }
    }
}