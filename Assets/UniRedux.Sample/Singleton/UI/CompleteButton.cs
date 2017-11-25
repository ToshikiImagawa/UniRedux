using UnityEngine.UI;

namespace UniRedux.Sample.Singleton.UI
{
    public abstract class CompleteButton : Button
    {
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

            CurrentStore?.Dispatch(new ToggleCompletedToDoAction
            {
                ToDoId = _toDoId
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
