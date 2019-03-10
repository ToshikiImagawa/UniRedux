using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Provider;
using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class ToDoView : IInitializable
    {
        [Inject]
        private UniReduxSignalBus uniReduxSignalBus;
        [Inject]
        private Transform _createPoint;
        [Inject]
        private ToDoElement.Pool _pool;

        private List<ToDoElement> _toDoElements = new List<ToDoElement>();

        private ToDo[] FilterToDos
        {
            get
            {
                var filter = UniReduxProvider.GetStore<ToDoState>().GetState().Filter;
                var toDos = UniReduxProvider.GetStore<ToDoState>().GetState().ToDos;
                switch (filter)
                {
                    case ToDoFilter.All:
                        return toDos;
                    case ToDoFilter.InProgress:
                        return toDos.Where(todo => !todo.Completed).ToArray();
                    case ToDoFilter.Completed:
                        return toDos.Where(todo => todo.Completed).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Initialize()
        {
            uniReduxSignalBus.Subscribe<ToDo[], ToDoState>(OnChange);
            uniReduxSignalBus.Subscribe<ToDoFilter, ToDoState>(OnChange);
        }

        private void UpdateDisplay()
        {
            var toDos = FilterToDos;
            for (var i = 0; i < toDos.Length; i++)
            {
                var toDo = toDos[i];
                var toDoElement = _toDoElements.Count > i ? _toDoElements[i] : _pool.Spawn(_createPoint);
                toDoElement.SetId(toDo.Id);
            }
            for (var i = _toDoElements.Count - 1; i >= toDos.Length; i--)
            {
                var toDoElement = _toDoElements[i];
                _pool.Despawn(toDoElement);
                _toDoElements.Remove(toDoElement);
            }
        }

        private void OnChange() { UpdateDisplay(); }
    }
}
