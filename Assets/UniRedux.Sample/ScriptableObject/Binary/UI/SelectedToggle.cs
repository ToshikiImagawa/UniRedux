using UniRedux.Sample.Application;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample3.UI
{
    public class SelectedToggle : Toggle
    {
        [SerializeField] private ToDoScriptableStore _toDoScriptableStore;
        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        private void Run(bool select)
        {
            DispachAction(select);
        }

        private void DispachAction(bool select)
        {
            if (_toDoId < 0) return;
            _toDoScriptableStore?.Dispatch(new UpdateSelectedToDoAction
            {
                ToDoId = _toDoId,
                IsSelected = select
            });
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