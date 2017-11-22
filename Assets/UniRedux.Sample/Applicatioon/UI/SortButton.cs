using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.Application.UI
{
    public class SortButton : Button
    {
        [SerializeField] private TodosFilter _filterType;

        private void Run()
        {
            DispachAction();
        }


        private void DispachAction()
        {
            ToDoApplication.CurrentStore.Dispatch(new ChangeToDosFilterAction
            {
                Filter = _filterType
            });
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}