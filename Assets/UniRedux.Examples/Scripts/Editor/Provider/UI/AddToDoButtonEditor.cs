using System.Reflection;
using UniRedux.Examples;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples.Editor
{
    [CustomEditor(typeof(AddToDoButton), true), CanEditMultipleObjects]
    public class AddToDoButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoMessageInputModule = serializedObject.FindProperty("_toDoMessageInputField");
            EditorGUILayout.PropertyField(toDoMessageInputModule, new GUIContent("ToDoMessageInputModule"));
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