using System.Reflection;
using UniRedux.Sample.ScriptableObject.Binary.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniReduxEditor.Sample.ScriptableObject.Binary.UI
{
    [CustomEditor(typeof(SelectedToggle)), CanEditMultipleObjects]
    public class SelectedToggleEditor : ToggleEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var toDoStore = serializedObject.FindProperty("_toDoScriptableStore");

            EditorGUILayout.PropertyField(toDoStore);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
        private void OnSceneGUI()
        {
            var editorGuiUtilityType = typeof(EditorGUIUtility);
            var icon = EditorGUIUtility.ObjectContent(null, typeof(Toggle)).image;
            var bindingFlags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] {target, icon};
            editorGuiUtilityType.InvokeMember("SetIconForObject", bindingFlags, null, null, args);
        }
    }
}