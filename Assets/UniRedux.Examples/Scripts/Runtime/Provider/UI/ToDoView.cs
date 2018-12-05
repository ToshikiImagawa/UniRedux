using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Examples;
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

        private ScrollRect ScrollRect
            => _scrollRect != null ? _scrollRect : (_scrollRect = GetComponent<ScrollRect>());

        private readonly IList<ToDoElement> _toDoElementObjectList = new List<ToDoElement>();

        private void Awake()
        {
            _disposable = ToDoApp.ToDoViewStateStateContainer.Inject(this);
            _factory = new ToDoElement.Factory(toDoElementPrefab);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        [UniReduxInject]
        private void Render([UniReduxInject(PropertyName = "ToDos")]
            Dictionary<int, ToDo> toDos, [UniReduxInject(PropertyName = "Filter")]
            ToDoFilter filter)
        {
            var toDoIds = toDos.GetFilterToDos(filter).Select(todo => todo.Id).ToArray();

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