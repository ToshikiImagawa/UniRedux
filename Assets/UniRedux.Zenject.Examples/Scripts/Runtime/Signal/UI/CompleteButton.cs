using UniRedux.Provider;
using UnityEngine.UI;

namespace UniRedux.Zenject.Examples.Signal
{
    public class CompleteButton : Button
    {
        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        private void Run()
        {
            DispatchAction();
        }
        private void DispatchAction()
        {
            if (_toDoId < 0) return;

            UniReduxProvider.Store.Dispatch(ToDoActionCreator.ToggleCompletedToDo(_toDoId));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}
