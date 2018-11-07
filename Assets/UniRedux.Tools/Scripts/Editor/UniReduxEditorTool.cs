using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Tools.Editor
{
    [InitializeOnLoad]
    public class UniReduxEditorTool
    {
        static UniReduxEditorTool()
        {
            // create unitypackage if compiled.
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                UnityPackage();
                ExamplesUnityPackage();
                ZenjectUnityPackage();
                ExamplesZenjectUnityPackage();
            }
        }

        [MenuItem("Window/UniRedux/Update ReduxPackage")]
        public static void UnityPackage()
        {
            var assetPathNames = new[] {"Assets/UniRedux"};
            const string fileName = "UniRedux.unitypackage";
            const ExportPackageOptions
                options = ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;

            AssetDatabase.ExportPackage(assetPathNames, fileName, options);
        }

        [MenuItem("Window/UniRedux/Update ExamplesReduxPackage")]
        public static void ExamplesUnityPackage()
        {
            var assetPathNames = new[] {"Assets/UniRedux.Examples"};
            const string fileName = "UniRedux.Examples.unitypackage";
            const ExportPackageOptions
                options = ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;

            AssetDatabase.ExportPackage(assetPathNames, fileName, options);
        }

        [MenuItem("Window/UniRedux/Zenject/Update ReduxPackage")]
        public static void ZenjectUnityPackage()
        {
            var assetPathNames = new[] {"Assets/UniRedux.Zenject"};
            const string fileName = "UniRedux.Zenject.unitypackage";
            const ExportPackageOptions
                options = ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;

            AssetDatabase.ExportPackage(assetPathNames, fileName, options);
        }

        [MenuItem("Window/UniRedux/Zenject/Update ExamplesReduxPackage")]
        public static void ExamplesZenjectUnityPackage()
        {
            var assetPathNames = new[] {"Assets/UniRedux.Zenject.Examples"};
            const string fileName = "UniRedux.Zenject.Examples.unitypackage";
            const ExportPackageOptions
                options = ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies;

            AssetDatabase.ExportPackage(assetPathNames, fileName, options);
        }
    }
}