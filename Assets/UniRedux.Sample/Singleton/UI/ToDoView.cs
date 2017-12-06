using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UniRedux.Sample.Singleton.UI
{
    public abstract class ToDoView : UIBehaviour, IObserver<ToDo[]>
    {
        [SerializeField] public ToDoElement _toDoElementPrefab;
        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();
        
        private ScrollRect _scrollRect;
        private ScrollRect ScrollRect => _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>());

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Debug.LogError(error);
        }

        public void OnNext(ToDo[] value)
        {
            var _toDoIds = value.Select(todo => todo.Id).ToArray();
            if (_toDoIds.Length < _toDoElementObjectList.Count)
            {
                for (var i = _toDoElementObjectList.Count - 1; i >= _toDoIds.Length; i--)
                {
                    var toDoElement = _toDoElementObjectList[i];
                    toDoElement.Dispose();
                    toDoElement.gameObject.SetActive(false);
                }
            }
            while (_toDoIds.Length > _toDoElementObjectList.Count)
            {
                var toDoElement = Instantiate(_toDoElementPrefab);
                toDoElement.transform.SetParent(ScrollRect.content.transform, false);
                _toDoElementObjectList.Add(toDoElement);
            }
            for (var i = 0; i < _toDoIds.Length; i++)
            {
                var toDoElement = _toDoElementObjectList[i];
                var toDoId = _toDoIds[i];

                toDoElement.gameObject.SetActive(true);
                toDoElement.Init(toDoId);
            }
        }

        protected override void Start()
        {
            ToDoSelector.FilterToDos(CurrentStore).Subscribe(this);
        }
        protected abstract IStore<ToDoState> CurrentStore { get; }
    }
}