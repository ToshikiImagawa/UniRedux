using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.Singleton.UI
{
    public abstract class SortButton : Button
    {
        [SerializeField] private TodosFilter _filterType;

        private void Run()
        {
            DispachAction();
        }

        private void DispachAction()
        {
            CurrentStore?.Dispatch(new ChangeToDosFilterAction
            {
                Filter = _filterType
            });
        }
        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
        protected abstract IStore<ToDoState> CurrentStore { get; }
    }
}
