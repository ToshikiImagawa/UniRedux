using System.Linq;
using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class ToDoElement : ReduxUIBehaviour<ToDoState>, IToDoListEventSystemHandler
    {
        private ToDo _toDo = new ToDo();

        private int ToDoId { get; set; } = -1;

        private Text _toDoTitle;

        private Text ToDoTitle => _toDoTitle != null
            ? _toDoTitle
            : (_toDoTitle = this.GetComponentFindNameInChildren<Text>("ToDoTitle"));

        private SelectedToggle _toDoSelector;

        private SelectedToggle ToDoSelectToggle
            => _toDoSelector != null
                ? _toDoSelector
                : (_toDoSelector = this.GetComponentFindNameInChildren<SelectedToggle>("Selector"));

        private CompleteButton _completeButton;

        private CompleteButton CompleteButton
            => _completeButton != null
                ? _completeButton
                : (_completeButton = this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton"));

        public void Init(int toDoId)
        {
            ToDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggle.Init(toDoId);
            CompleteButton.Init(toDoId);

            var toDo = State.ToDos.ContainsKey(ToDoId) ? State.ToDos[ToDoId] : null;
            if (toDo == null) return;
            UpdateView(toDo);
        }

        private void HandleChange()
        {
            var toDo = State.ToDos.ContainsKey(ToDoId) ? State.ToDos[ToDoId] : null;
            if (toDo == null) return;

            if (_toDo.Id == toDo.Id &&
                _toDo.Completed == toDo.Completed &&
                _toDo.Selected == toDo.Selected &&
                _toDo.Text == toDo.Text) return;

            _toDo = toDo;
            UpdateView(toDo);
        }

        private void UpdateView(ToDo value)
        {
            var toDoText = value.Text;
            var isCompleted = value.Completed;
            var isSelected = value.Selected;

            ToDoTitle.text = toDoText;
            CompleteButton.targetGraphic.color = isCompleted
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            ToDoTitle.color = isCompleted
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            ToDoSelectToggle.isOn = isSelected;
        }

        public void OnRegisterComponent()
        {
            HandleChange();
        }

        public void OnChangeToDoList()
        {
            HandleChange();
        }

        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}