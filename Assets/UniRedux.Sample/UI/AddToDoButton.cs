using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.UI
{
    public class AddToDoButton : Button
    {
        [SerializeField] private InputField _toDoInputField;

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
            ToDoApplication.CurrentStore.Dispatch(new AddToDoAction
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
    }
}