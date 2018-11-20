using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using System;

namespace UniRedux.Zenject.Examples.Simple
{
    public class ToDoView : UIBehaviour
    {
        [Inject] private ToDoElement.Pool _toDoElementPool;
        [Inject] private IStore<ToDoState> _store;

        private IDisposable _disposable;

        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        private ScrollRect _scrollRect;
        private ScrollRect ScrollRect => _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>());

        private int[] _toDoIds = new int[0];

        protected override void Awake()
        {
            _disposable = _store?.Subscribe(state => { HandleChange(); });
        }

        protected override void OnDestroy()
        {
            _disposable.Dispose();
        }

        private void HandleChange()
        {
            var toDoIds = _store.GetState().GetFilterToDos().Select(todo => todo.Id).ToArray();
            if (toDoIds.SequenceEqual(_toDoIds)) return;
            _toDoIds = toDoIds;

            if (_toDoIds.Length < _toDoElementObjectList.Count)
            {
                for (var i = _toDoElementObjectList.Count - 1; i >= _toDoIds.Length; i--)
                {
                    var toDoElement = _toDoElementObjectList[i];
                    toDoElement.gameObject.SetActive(false);
                    _toDoElementPool.Despawn(toDoElement);
                    _toDoElementObjectList.Remove(toDoElement);
                }
            }

            while (_toDoIds.Length > _toDoElementObjectList.Count)
            {
                _toDoElementObjectList.Add(_toDoElementPool.Spawn(ScrollRect.content.transform));
            }

            for (var i = 0; i < _toDoIds.Length; i++)
            {
                var toDoElement = _toDoElementObjectList[i];
                var toDoId = _toDoIds[i];

                toDoElement.gameObject.SetActive(true);
                toDoElement.Init(toDoId);
            }
        }
    }
}