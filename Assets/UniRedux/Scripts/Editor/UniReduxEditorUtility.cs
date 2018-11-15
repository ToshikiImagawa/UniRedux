using System.IO;
using UnityEngine;

namespace UniRedux.Editor
{
    public static class UniReduxEditorUtility
    {
        [SerializeField]
        private static string _packagePath;

        [SerializeField]
        private static string _packageFullPath;

        private static string _folderPath = "Not Found";

        /// <summary>
        /// Returns the relative path of the package.
        /// </summary>
        public static string PackageRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(_packagePath))
                    _packagePath = GetPackageRelativePath();

                return _packagePath;
            }
        }

        /// <summary>
        /// Returns the fully qualified path of the package.
        /// </summary>
        public static string PackageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(_packageFullPath))
                    _packageFullPath = GetPackageFullPath();

                return _packageFullPath;
            }
        }

        private static string GetPackageRelativePath()
        {
            string packagePath = Path.GetFullPath("Packages/com.comcreate-info.uni_redux");
            if (Directory.Exists(packagePath))
            {
                return "Packages/com.comcreate-info.uni_redux";
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                if (Directory.Exists(packagePath + "/Assets/Packages/com.comcreate-info.uni_redux/Editor Resources"))
                {
                    return "Assets/Packages/com.comcreate-info.uni_redux";
                }

                if (Directory.Exists(packagePath + "/Assets/UniRedux/Editor Resources"))
                {
                    return "Assets/UniRedux";
                }

                string[] matchingPaths = Directory.GetDirectories(packagePath, "UniRedux", SearchOption.AllDirectories);
                packagePath = ValidateLocation(matchingPaths, packagePath);
                if (packagePath != null) return packagePath;
            }

            return null;
        }

        private static string GetPackageFullPath()
        {
            string packagePath = Path.GetFullPath("Packages/com.comcreate-info.uni_redux");
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }

            packagePath = Path.GetFullPath("Assets/..");
            if (Directory.Exists(packagePath))
            {
                if (Directory.Exists(packagePath + "/Assets/Packages/com.comcreate-info.uni_redux/Editor Resources"))
                {
                    return packagePath + "/Assets/Packages/com.comcreate-info.uni_redux";
                }
                if (Directory.Exists(packagePath + "/Assets/UniRedux/Editor Resources"))
                {
                    return packagePath + "/Assets/UniRedux";
                }
                string[] matchingPaths = Directory.GetDirectories(packagePath, "UniRedux", SearchOption.AllDirectories);
                string path = ValidateLocation(matchingPaths, packagePath);
                if (path != null) return packagePath + path;
            }

            return null;
        }

        private static string ValidateLocation(string[] paths, string projectPath)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                if (Directory.Exists(paths[i] + "/Editor Resources"))
                {
                    _folderPath = paths[i].Replace(projectPath, "");
                    _folderPath = _folderPath.TrimStart('\\', '/');
                    return _folderPath;
                }
            }

            return null;
        }
    }
}
