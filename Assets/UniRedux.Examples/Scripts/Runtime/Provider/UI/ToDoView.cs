using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Examples;
using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    [RequireComponent(typeof(ScrollRect))]
    public class ToDoView : MonoBehaviour, IUniReduxComponent
    {
        [SerializeField] private ToDoElement toDoElementPrefab;

        private IDisposable _disposable;

        private ScrollRect _scrollRect;
        private ToDoElement.Factory _factory;
        private Dictionary<int, ToDo> _toDos;
        private ToDoFilter _filter;
        
        [UniReduxInject]
        private Dictionary<int, ToDo> ToDos
        {
            get { return _toDos; }
            set
            {
                _toDos = value;
                Render();
            }
        }
        
        [UniReduxInject]
        private ToDoFilter Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                Render();
            }
        }
        

        private ScrollRect ScrollRect
            => _scrollRect != null ? _scrollRect : (_scrollRect = GetComponent<ScrollRect>());

        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        private void Awake()
        {
            _factory = new ToDoElement.Factory(toDoElementPrefab);
            _disposable = ToDoApp.ToDoViewStateStateContainer.Inject(this);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        private void Render()
        {
            var toDoIds = ToDos.GetFilterToDos(Filter).Select(todo => todo.Id).ToArray();

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
                var toDoElement = _factory.Create(ScrollRect.content.transform);
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