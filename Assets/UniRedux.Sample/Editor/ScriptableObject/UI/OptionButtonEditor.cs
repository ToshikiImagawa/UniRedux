﻿using System.Reflection;
using UniRedux.Sample.ScriptableObject.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniReduxEditor.Sample.ScriptableObject.UI
{
    [CustomEditor(typeof(OptionButton)), CanEditMultipleObjects]
    public class OptionButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoStore = serializedObject.FindProperty("_toDoApplicationObject");
            var toDoInputField = serializedObject.FindProperty("_optionType");

            EditorGUILayout.PropertyField(toDoStore);
            EditorGUILayout.PropertyField(toDoInputField);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(Button)).image;
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { target, icon };
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
    }
}
