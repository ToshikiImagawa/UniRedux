using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.UI
{
    public class SortButton : Button
    {
        [SerializeField] private UnityEngine.ScriptableObject _toDoApplicationObject;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
        [SerializeField] private TodosFilter _filterType;

        private void Run()
        {
            DispachAction();
        }

        private void DispachAction()
        {
            ToDoApplication?.CurrentStore?.Dispatch(new ChangeToDosFilterAction
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
