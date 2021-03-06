﻿using System.Linq;
using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.UI
{
    public class ToDoElement : RxUIBehaviour<ToDoState>
    {
        [SerializeField] private UnityEngine.ScriptableObject _toDoApplicationObject;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
        private ToDo _toDo = new ToDo();

        public int ToDoId { get; private set; } = -1;

        private Text _toDoTitle;
        private Text ToDoTitle => _toDoTitle ?? (_toDoTitle = this.GetComponentFindNameInChildren<Text>("ToDoTitle"));
        private SelectedToggle _toDoSelecter;

        private SelectedToggle ToDoSelectToggle
            => _toDoSelecter ?? (_toDoSelecter = this.GetComponentFindNameInChildren<SelectedToggle>("Selecter"));

        private CompleteButton _completeButton;

        private CompleteButton CompleteButton
            =>
                _completeButton ??
                (_completeButton = this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton"));

        public void Init(int toDoId)
        {
            ToDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggle.Init(toDoId);
            CompleteButton.Init(toDoId);

            var toDo = State.GetFilterToDos().FirstOrDefault(todo => todo.Id == ToDoId);
            if (toDo == null) return;
            UpdateView(toDo);
        }

        private void HandleChange()
        {
            var toDo = State.GetFilterToDos().FirstOrDefault(todo => todo.Id == ToDoId);
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

        protected override IStore<ToDoState> CurrentStore => ToDoApplication.CurrentStore;
    }
}