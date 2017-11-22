﻿using System.Reflection;
using UniRedux.Sample3.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniReduxEditor.Sample3.UI
{
    [CustomEditor(typeof(OptionButton)), CanEditMultipleObjects]
    public class OptionButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoInputField = serializedObject.FindProperty("_optionType");
            var toDoStore = serializedObject.FindProperty("_toDoScriptableStore");

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