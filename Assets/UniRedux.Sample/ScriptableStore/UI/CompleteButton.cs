using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableStore.UI
{
    public class CompleteButton : Button
    {
        [SerializeField] private ScriptableObject _toDoApplicationObject;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
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

            ToDoApplication?.CurrentStore?.Dispatch(new ToggleCompletedToDoAction
            {
                ToDoId = _toDoId
            });
        }
        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            if (ToDoApplication == null) _toDoApplicationObject = null;
        }
    }
}