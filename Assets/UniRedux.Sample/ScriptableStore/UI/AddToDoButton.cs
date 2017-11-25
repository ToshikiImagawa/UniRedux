using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableStore.UI
{
    public class AddToDoButton : Button
    {
        [SerializeField] private ScriptableObject _toDoApplicationObject;
        [SerializeField] private InputField _toDoInputField;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;

        private void Run()
        {
            if (_toDoInputField == null) return;
            var text = _toDoInputField.text;

            if (!string.IsNullOrEmpty(text))
            {
                DispachAction(text);
            }
        }

        private void DispachAction(string text)
        {
            ToDoApplication?.CurrentStore?.Dispatch(new AddToDoAction
            {
                Text = text
            });
            _toDoInputField.text = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (ToDoApplication == null) _toDoApplicationObject = null;
        }
    }
}