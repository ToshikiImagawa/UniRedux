using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.EventSystems;
using UniRedux.Examples.Application;
using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public abstract class ToDoViewBase : ReduxBehaviour<ToDoState>, IToDoListEventSystemHandler,
        IToDoFilterEventSystemHandler
    {
        private readonly IList<ToDoElementBase> _toDoElementObjectList = new List<ToDoElementBase>();
        private Dictionary<int, ToDo> _toDos = new Dictionary<int, ToDo>();
        private IDisposable _disposable;

        protected abstract Func<Transform, ToDoElementBase> ToDoElementFactory { get; }

        public void OnRegisterComponent()
        {
            UpdateAll();
        }

        public void OnChangeToDoFilter()
        {
            UpdateDisplay();
        }

        public void OnChangeToDoList()
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            _toDos = CurrentStore.GetState().ToDos;
            UpdateDisplay();
        }

        protected override void AfterInit()
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        private ScrollRect _scrollRect;

        private ScrollRect ScrollRect
            => _scrollRect != null ? _scrollRect : (_scrollRect = GetComponent<ScrollRect>());

        private void UpdateDisplay()
        {
            var toDoIds = _toDos.GetFilterToDos(CurrentStore.GetState().Filter).Select(todo => todo.Id).ToArray();

            if (toDoIds.Length < _toDoElementObjectList.Count)
            {
                for (var i = _toDoElementObjectList.Count - 1; i >= toDoIds.Length; i--)
                {
                    var toDoElement = _toDoElementObjectList[i];
                    toDoElement.gameObject.SetActive(false);
                }
            }

            while (toDoIds.Length > _toDoElementObjectList.Count)
            {
                var toDoElement = ToDoElementFactory(ScrollRect.content.transform);
                _toDoElementObjectList.Add(toDoElement);
            }

            for (var i = 0; i < toDoIds.Length; i++)
            {
                var toDoElement = _toDoElementObjectList[i];
                var toDoId = toDoIds[i];

                toDoElement.gameObject.SetActive(true);
                toDoElement.Init(toDoId);
            }
        }
    }
}