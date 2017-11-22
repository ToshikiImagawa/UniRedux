using UniRedux.Sample.Application;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.Json.UI
{
    public class SortButton : Button
    {
        [SerializeField] private ToDoScriptableStore _toDoScriptableStore;
        [SerializeField] private TodosFilter _filterType;

        private void Run()
        {
            DispachAction();
        }

        private void DispachAction()
        {
            _toDoScriptableStore?.Dispatch(new ChangeToDosFilterAction
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
