using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class ToDoView : MonoBehaviour, IReduxEventSystemSubscriber, IToDoListEventSystemHandler,
        IToDoFilterEventSystemHandler
    {
        [SerializeField] private ToDoElement _toDoElementPrefab;
        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();
        private Dictionary<int, ToDo> _toDos = new Dictionary<int, ToDo>();
        private IDisposable _disposable;

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

        void IDisposable.Dispose()
        {
            _disposable?.Dispose();
        }

        void IReduxEventSystemSubscriber.SubscribeEventSystem(IDisposable disposable)
        {
            _disposable = disposable;
        }

        private void UpdateAll()
        {
            _toDos = CurrentStore.GetState().ToDos;
            UpdateDisplay();
        }

        private void Awake()
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
                var toDoElement = Instantiate(_toDoElementPrefab);
                toDoElement.transform.SetParent(ScrollRect.content.transform, false);
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

        private static IStore<ToDoState> CurrentStore 
            => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}