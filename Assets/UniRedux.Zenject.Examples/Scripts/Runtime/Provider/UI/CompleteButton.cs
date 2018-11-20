using UniRedux.Provider;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class CompleteButton : Button, IUniReduxComponent
    {
        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }

        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }


        private void Run()
        {
            DispatchAction();
        }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            container.Inject(this);
        }

        private void DispatchAction()
        {
            if (_toDoId < 0) return;

            Dispatch(ToDoActionCreator.ToggleCompletedToDo(_toDoId));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}