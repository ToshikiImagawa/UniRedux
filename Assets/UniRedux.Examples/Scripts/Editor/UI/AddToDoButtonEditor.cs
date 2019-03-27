using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples.EventSystem.Editor
{
    [CustomEditor(typeof(AddToDoButtonBase), true), CanEditMultipleObjects]
    public class AddToDoButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoMessageInputModuleObject = serializedObject.FindProperty("_toDoMessageInputModuleObject");
            var toDoMessageInputModule = serializedObject.FindProperty("_toDoMessageInputModule");

            if (toDoMessageInputModuleObject?.objectReferenceValue != null)
            {
                if (toDoMessageInputModuleObject.objectReferenceValue.GetType() != typeof(GameObject))
                {
                    toDoMessageInputModuleObject.objectReferenceValue = null;
                }
                else
                {
                    var gameObject = (GameObject) toDoMessageInputModuleObject.objectReferenceValue;
                    toDoMessageInputModule.objectReferenceValue =
                        gameObject.GetComponent(typeof(IToDoMessageInputModule));
                    if (toDoMessageInputModule.objectReferenceValue == null)
                        toDoMessageInputModuleObject.objectReferenceValue = null;
                }
            }

            EditorGUILayout.PropertyField(toDoMessageInputModuleObject, new GUIContent("ToDoMessageInputModule"));
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