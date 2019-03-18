using UniRedux.Provider;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class SelectedToggle : Toggle
    {
        [Inject] private UniReduxSignalBus _uniReduxSignalBus;

        [Inject] private ToDoElement _toDoElement;

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run(!isOn);
        }

        private void Run(bool select)
        {
            DispatchAction(select);
        }

        private void DispatchAction(bool select)
        {
            if (_toDoElement.ToDoId < 0) return;
            UniReduxProvider.Store.Dispatch(ToDoActionCreator.UpdateSelectedToDo(_toDoElement.ToDoId, select));
        }
    }
}