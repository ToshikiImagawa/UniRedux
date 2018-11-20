using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Simple
{
    public class CompleteButton : Button
    {
        [Inject]
        private IStore<ToDoState> _store;

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
            if (_store == null) return;
            if (_toDoId < 0) return;

            _store.Dispatch(ToDoActionCreator.ToggleCompletedToDo(_toDoId));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}