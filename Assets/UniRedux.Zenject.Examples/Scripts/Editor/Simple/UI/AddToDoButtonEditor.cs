using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniRedux.Zenject.Examples.Simple.Editor
{
    [CustomEditor(typeof(AddToDoButton)), CanEditMultipleObjects]
    public class AddToDoButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoInputField = serializedObject.FindProperty("_toDoInputField");

            if (toDoInputField != null) EditorGUILayout.PropertyField(toDoInputField);
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