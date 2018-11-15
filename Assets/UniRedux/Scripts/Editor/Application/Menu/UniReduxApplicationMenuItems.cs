using System.Linq;
using UnityEditor;

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
    }
}