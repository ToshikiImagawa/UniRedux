using UnityEngine.UI;

namespace UniRedux.Sample.UI
{
    public class SelectedToggle : Toggle
    {
        private int _toDoId = -1;

        protected override void Awake()
        {
            base.Awake();
            onValueChanged.AddListener(Run);
        }

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        private void Run(bool select)
        {
            DispachAction(select);
        }

        private void DispachAction(bool select)
        {
            if (_toDoId < 0) return;
            ToDoApplication.CurrentStore.Dispatch(new UpdateSelectedToDoAction
            {
                ToDoId = _toDoId,
                IsSelected = select
            });
        }
    }
}