using UniRedux.EventSystems;
using UniRedux.Examples.EventSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public abstract class ToDoElementBase : ReduxUIBehaviour<ToDoState>, IToDoListEventSystemHandler
    {
        private ToDo _toDo = new ToDo();

        private int ToDoId { get; set; } = -1;

        private Text _toDoTitle;

        private Text ToDoTitle => _toDoTitle != null
            ? _toDoTitle
            : (_toDoTitle = this.GetComponentFindNameInChildren<Text>("ToDoTitle"));

        private SelectedToggleBase _toDoSelector;

        private SelectedToggleBase ToDoSelectToggleBase
            => _toDoSelector != null
                ? _toDoSelector
                : (_toDoSelector = this.GetComponentFindNameInChildren<SelectedToggleBase>("Selector"));

        private CompleteButtonBase _completeButton;

        private CompleteButtonBase CompleteButton
            => _completeButton != null
                ? _completeButton
                : (_completeButton = this.GetComponentFindNameInChildren<CompleteButtonBase>("CompleteButton"));

        public void Init(int toDoId)
        {
            ToDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggleBase.Init(toDoId);
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
            ToDoSelectToggleBase.isOn = isSelected;
        }

        public void OnRegisterComponent()
        {
            HandleChange();
        }

        public void OnChangeToDoList()
        {
            HandleChange();
        }
    }
}