using UniRedux;
using UniRedux.Sample;
using UnityEditor;
using UnityEngine;

namespace UniReduxEditor.Sample
{
    public class StateDisplay : EditorWindow
    {
        private string textArea;

        [MenuItem("UniRedux/StateDisplay open")]
        static void Init()
        {
            StateDisplay window = (StateDisplay) GetWindow(typeof(StateDisplay));
            window.Show();
        }

        void OnGUI()
        {
            EditorGUILayout.PrefixLabel("StateDisplay:");
            EditorStyles.textField.wordWrap = true;
            EditorGUILayout.TextArea(textArea);

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
                    textArea = toDoState.ToJson();
                }
            }
            catch
            {
                textArea = string.Empty;
            }
        }
    }
}