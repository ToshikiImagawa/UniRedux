using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniRedux.Zenject.Examples.Editor
{
    [CustomEditor(typeof(SortButton)), CanEditMultipleObjects]
    public class SortButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoStore = serializedObject.FindProperty("_toDoApplicationObject");
            var filterType = serializedObject.FindProperty("_filterType");

            if (toDoStore != null) EditorGUILayout.PropertyField(toDoStore);
            if (filterType != null) EditorGUILayout.PropertyField(filterType);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(Button)).image;
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] {target, icon};
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
    }
}