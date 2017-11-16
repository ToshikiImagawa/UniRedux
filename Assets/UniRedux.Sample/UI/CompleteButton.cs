using UnityEngine.UI;

namespace UniRedux.Sample.UI
{
    public class CompleteButton : Button

    {
        private int _toDoId = -1;


        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(Run);
        }


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

            ToDoApplication.CurrentStore.Dispatch(new ToggleCompletedToDoAction
            {
                ToDoId = _toDoId
            });
        }
    }
}