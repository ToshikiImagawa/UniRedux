using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Simple
{
    public class SelectedToggle : Toggle
    {
        [Inject]
        private IStore<ToDoState> _store;
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
            if (_store == null) return;
            if (_toDoId < 0) return;
            _store.Dispatch(ToDoActionCreator.UpdateSelectedToDo(_toDoId, select));
        }

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run(!isOn);
        }
    }
}