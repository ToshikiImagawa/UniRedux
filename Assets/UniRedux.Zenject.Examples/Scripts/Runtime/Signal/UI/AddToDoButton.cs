using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Zenject.Examples.Signal
{
    public class AddToDoButton : Button
    {
        [SerializeField] private InputField toDoInputField;

        private void Run()
        {
            if (toDoInputField == null) return;
            var text = toDoInputField.text;

            if (!string.IsNullOrEmpty(text))
            {
                DispatchAction(text);
            }
        }

        private void DispatchAction(string text)
        {
            UniReduxProvider.Store.Dispatch(ToDoActionCreator.AddToDo(text));
            toDoInputField.text = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}
