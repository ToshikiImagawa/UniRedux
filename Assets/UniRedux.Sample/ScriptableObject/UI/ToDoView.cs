using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.UI
{
    public class ToDoView : RxUIBehaviour<ToDoState>
    {
        [SerializeField] private UnityEngine.ScriptableObject _toDoApplicationObject;
        [SerializeField] public ToDoElement _toDoElementPrefab;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        private ScrollRect _scrollRect;
        private ScrollRect ScrollRect => _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>());

        private int[] _toDoIds = new int[0];

        private void HandleChange()
        {
            var toDoIds = State.GetFilterToDos().Select(todo => todo.Id).ToArray();
            if (toDoIds.SequenceEqual(_toDoIds)) return;
            _toDoIds = toDoIds;

            if (_toDoIds.Length < _toDoElementObjectList.Count)
            {
                for (var i = _toDoElementObjectList.Count - 1; i >= _toDoIds.Length; i--)
                {
                    var toDoElement = _toDoElementObjectList[i];
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

        protected override IStore<ToDoState> CurrentStore => ToDoApplication.CurrentStore;
    }
}