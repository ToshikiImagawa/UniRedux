using UniRedux.Sample.Application;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample3.UI
{
    public class AddToDoButton : Button
    {
        [SerializeField] private ToDoScriptableStore _toDoScriptableStore;
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
            _toDoScriptableStore?.Dispatch(new AddToDoAction
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