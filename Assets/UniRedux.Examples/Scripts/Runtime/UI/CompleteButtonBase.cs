﻿using UnityEngine.UI;

namespace UniRedux.Examples
{
    public abstract class CompleteButtonBase : Button
    {
        private int _toDoId = -1;

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        private void DispatchAction()
        {
            if (_toDoId < 0) return;

            CurrentStore?.Dispatch(ToDoActionCreator.ToggleCompletedToDo(_toDoId));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            DispatchAction();
            base.OnPointerDown(eventData);
        }

        protected abstract IStore<ToDoState> CurrentStore { get; }
    }
}