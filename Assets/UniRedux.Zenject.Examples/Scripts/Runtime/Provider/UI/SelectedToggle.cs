using UniRedux.Provider;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class SelectedToggle : Toggle, IUniReduxComponent
    {
        private int _toDoId = -1;

        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }

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
            Dispatch(ToDoActionCreator.UpdateSelectedToDo(_toDoId, select));
        }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            container.Inject(this);
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