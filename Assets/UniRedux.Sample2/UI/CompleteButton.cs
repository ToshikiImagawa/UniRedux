using UniRedux.Sample;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample2.UI
{
    public class CompleteButton : Button
    {
        [SerializeField] private ToDoScriptableStore _toDoScriptableStore;
        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }


        private void Run()
        {
            DispachAction();
        }


        private void DispachAction()
        {
            if (_toDoId < 0) return;

            _toDoScriptableStore?.Dispatch(new ToggleCompletedToDoAction
            {
                ToDoId = _toDoId
            });
        }
        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}