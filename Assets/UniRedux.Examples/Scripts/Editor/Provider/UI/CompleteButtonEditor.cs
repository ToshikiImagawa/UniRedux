using System.Reflection;
using UniRedux.Examples;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples.Editor
{
    [CustomEditor(typeof(CompleteButton), true), CanEditMultipleObjects]
    public class CompleteButtonEditor : ButtonEditor
    {
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