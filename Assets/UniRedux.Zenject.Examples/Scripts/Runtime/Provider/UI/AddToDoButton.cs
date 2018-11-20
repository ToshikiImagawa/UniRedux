using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class AddToDoButton : Button, IUniReduxComponent
    {
        [SerializeField] private InputField toDoInputField;

        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            container.Inject(this);
        }

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
            Dispatch(ToDoActionCreator.AddToDo(text));
            toDoInputField.text = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}