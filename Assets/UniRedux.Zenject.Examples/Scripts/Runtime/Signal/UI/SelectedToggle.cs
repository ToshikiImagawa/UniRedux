using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class SelectedToggle : Toggle
    {
        [Inject]
        private UniReduxSignalBus _uniReduxSignalBus;

        private ToDo _toDo;

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run(!isOn);
        }

        protected override void Awake()
        {
            base.Awake();
            _uniReduxSignalBus.Subscribe<ToDo>(OnChange);
        }

        private void Run(bool select)
        {
            DispatchAction(select);
        }

        private void DispatchAction(bool select)
        {
            if (_toDo == null || _toDo.Id < 0) return;
            UniReduxProvider.Store.Dispatch(ToDoActionCreator.UpdateSelectedToDo(_toDo.Id, select));
        }

        private void OnChange(ToDo toDo)
        {
            _toDo = toDo;
            if (_toDo == null || _toDo.Id < 0) return;
            isOn = _toDo.Selected;
        }
    }
}
