using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Simple
{
    public class AddToDoButton : Button
    {
        [SerializeField] private InputField _toDoInputField;

        [Inject]
        private IStore<ToDoState> _store;

        private void Run()
        {
            if (_toDoInputField == null) return;
            var text = _toDoInputField.text;

            if (!string.IsNullOrEmpty(text))
            {
                DispatchAction(text);
            }
        }

        private void DispatchAction(string text)
        {
            if (_store == null) return;
            _store.Dispatch(ToDoActionCreator.AddToDo(text));
            _toDoInputField.text = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}