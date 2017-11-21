using System.Reflection;
using UniRedux.Sample.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.UI;

namespace UniReduxEditor.Sample.UI
{
    [CustomEditor(typeof(SelectedToggle)), CanEditMultipleObjects]
    public class SelectedToggleEditor : ToggleEditor
    {
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