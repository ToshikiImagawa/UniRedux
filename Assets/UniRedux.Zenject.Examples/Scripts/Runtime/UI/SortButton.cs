using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples
{
    public class SortButton : Button
    {
        [SerializeField] private ToDoFilter _filterType;
        [Inject]
        private IStore<ToDoState> _store;
        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            if (_store == null) return;
            _store.Dispatch(ToDoActionCreator.ChangeToDoFilter(_filterType));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}