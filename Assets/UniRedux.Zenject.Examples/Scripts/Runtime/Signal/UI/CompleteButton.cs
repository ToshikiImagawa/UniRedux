using UniRedux.Provider;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class CompleteButton : Button
    {
        [Inject]
        private ToDoElement _element;

        private void Run()
        {
            DispatchAction();
        }
        private void DispatchAction()
        {
            if (_element.ToDoId < 0) return;

            UniReduxProvider.Store.Dispatch(ToDoActionCreator.ToggleCompletedToDo(_element.ToDoId));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}
