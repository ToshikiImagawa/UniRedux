﻿using System.Reflection;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniRedux.Examples.Editor
{
    [CustomEditor(typeof(SelectedToggleBase), true), CanEditMultipleObjects]
    public class SelectedToggleEditor : ToggleEditor
    {
        private void OnSceneGUI()
        {
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(Toggle)).image;
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { target, icon };
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
    }
}