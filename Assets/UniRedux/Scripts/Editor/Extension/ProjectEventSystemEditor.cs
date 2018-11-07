using System.Reflection;
using UniRedux.EventSystems;
using UnityEditor;
using UnityEngine;

namespace UniRedux.Editor
{
    [CustomEditor(typeof(ProjectEventSystem))]
    public class ProjectEventSystemEditor : UnityEditor.Editor
    {
        private bool _foldoutStatus = true;

        public override void OnInspectorGUI()
        {
            var rxEventSystem = target as ProjectEventSystem;
            _foldoutStatus = EditorGUILayout.Foldout(_foldoutStatus, "TargetGameObjects");
            if (!_foldoutStatus || rxEventSystem == null) return;
            foreach (var targetObject in rxEventSystem.TargetObjects)
            {
                EditorGUILayout.ObjectField(targetObject, typeof(GameObject), true);
            }
        }

        private void OnSceneGUI()
        {
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(UnityEngine.EventSystems.EventSystem)).image;
            const BindingFlags bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] {target, icon};
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
    }
}