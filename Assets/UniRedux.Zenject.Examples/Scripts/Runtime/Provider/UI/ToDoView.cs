using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Provider;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class ToDoView : UIBehaviour, IUniReduxComponent
    {
        [Inject] private ToDoElement.Pool _toDoElementPool;

        private IDisposable _disposable;

        private ScrollRect _scrollRect;
        private ToDo[] _toDos;
        private ToDoFilter _filter;

        [UniReduxInject]
        private ToDo[] FilterToDos
        {
            get { return _toDos; }
            set
            {
                _toDos = value;
                Render();
            }
        }

        private ScrollRect ScrollRect
            => _scrollRect != null ? _scrollRect : (_scrollRect = GetComponent<ScrollRect>());

        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            _disposable = container.Inject(this);
        }

        protected override void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void Render()
        {
            var toDoIds = FilterToDos.Select(todo => todo.Id).ToArray();

            if (toDoIds.Length < _toDoElementObjectList.Count)
            {
                for (var i = _toDoElementObjectList.Count - 1; i >= toDoIds.Length; i--)
                {
                    var toDoElement = _toDoElementObjectList[i];
                    toDoElement.gameObject.SetActive(false);
                    _toDoElementPool.Despawn(toDoElement);
                    _toDoElementObjectList.Remove(toDoElement);
                }
            }

            while (toDoIds.Length > _toDoElementObjectList.Count)
            {
                var toDoElement = _toDoElementPool.Spawn(ScrollRect.content.transform);
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