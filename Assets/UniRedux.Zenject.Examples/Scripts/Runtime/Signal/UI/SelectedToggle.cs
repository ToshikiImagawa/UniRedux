using UniRedux.Provider;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class SelectedToggle : Toggle
    {
        [Inject]
        private ToDoElement _element;
        
        private void Run(bool select)
        {
            DispatchAction(select);
        }

        private void DispatchAction(bool select)
        {
            if (_element.ToDoId < 0) return;
            UniReduxProvider.Store.Dispatch(ToDoActionCreator.UpdateSelectedToDo(_element.ToDoId, select));
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
