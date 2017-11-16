using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace UniRedux.Sample
{
    public class ToDoView : UIBehaviour, IObserver<ToDo[]>
    {
        [SerializeField] public ToDoElement _toDoElementPrefab;
        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        private int[] _toDoIds = Util.Empty<int>();

        private ScrollRect _scrollRect;
        private ScrollRect ScrollRect => _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>());

        public void OnCompleted()
        {
            var toDoIdQeue = new Queue<int>(_toDoIds);
            foreach (var toDoElement in _toDoElementObjectList)
            {
                if (toDoIdQeue.Count > 0)
                {
                    toDoElement.gameObject.SetActive(true);
                    var toDoId = toDoIdQeue.Dequeue();

                    if(toDoElement.ToDoId != toDoId) toDoElement.Init(toDoId);
                }
                else
                {
                    toDoElement.Dispose();
                    toDoElement.gameObject.SetActive(false);
                }
            }
            while (toDoIdQeue.Count>0)
            {
                var toDoId = toDoIdQeue.Dequeue();
                var toDoElement = Instantiate(_toDoElementPrefab);
                toDoElement.transform.SetParent(ScrollRect.content.transform, false);
                _toDoElementObjectList.Add(toDoElement);
                if (toDoElement.ToDoId != toDoId) toDoElement.Init(toDoId);
            }
        }

        public void OnError(Exception error)
        {
            Debug.LogError(error);
        }

        public void OnNext(ToDo[] value)
        {
            _toDoIds = value.Select(todo => todo.Id).ToArray();
        }

        protected override void Start()
        {
            ToDoSelector.FilterToDos().Subscribe(this);
        }
    }
}