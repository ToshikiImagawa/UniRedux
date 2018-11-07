using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class SelectedToggle : Toggle
    {
        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        private void Run(bool select)
        {
            DispatchAction(select);
        }

        private void DispatchAction(bool select)
        {
            if (_toDoId < 0) return;
            CurrentStore?.Dispatch(ToDoActionCreator.UpdateSelectedToDo(_toDoId, select));
        }

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run(!isOn);
        }

        private IStore<ToDoState> CurrentStore => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}