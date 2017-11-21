using UniRedux;
using UniRedux.Sample;
using UnityEditor;
using UnityEngine;

namespace UniReduxEditor.Sample
{
    public class StateDisplay : EditorWindow
    {
        private string toDoStateJson;

        [MenuItem("UniRedux/ToDoList/StateDisplay open")]
        private static void Init()
        {
            var window = (StateDisplay) GetWindow(typeof(StateDisplay));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.PrefixLabel("StateDisplay:");
            EditorStyles.textField.wordWrap = true;
            EditorGUILayout.SelectableLabel(toDoStateJson, EditorStyles.textArea, GUILayout.ExpandHeight(true));

            if (GUILayout.Button("Display"))
                Display();

            if (GUILayout.Button("Close"))
                Close();
        }

        private void Display()
        {
            try
            {
                var toDoState = ToDoApplication.CurrentStore.GetState();
                if (toDoState != null)
                {
                    toDoStateJson = toDoState.ToJson();
                }
            }
            catch
            {
                toDoStateJson = string.Empty;
            }
        }
    }
}