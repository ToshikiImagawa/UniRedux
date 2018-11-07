﻿using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class SortButton : Button
    {
        [SerializeField] private ToDoFilter _filterType;

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            CurrentStore?.Dispatch(ToDoActionCreator.ChangeToDoFilter(_filterType));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        private IStore<ToDoState> CurrentStore => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}