using System.Linq;
using UnityEditor;

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
    }
}