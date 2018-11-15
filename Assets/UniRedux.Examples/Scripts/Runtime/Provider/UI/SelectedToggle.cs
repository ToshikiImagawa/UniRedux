using System;
using System.Collections.Generic;
using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    public class SelectedToggle : Toggle, IUniReduxComponent
    {
        private IDisposable _disposable;
        private int ToDoId { get; set; } = -1;

        private ToDoFilter _filter;
        private Dictionary<int, ToDo> _toDos;

        [UniReduxInject]
        private Action<int, bool> UpdateSelectedToDo { get; set; }

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

        public void Init(int toDoId)
        {
            ToDoId = toDoId;
            Render();
        }

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            _disposable = ToDoApp.ToDoViewStateStateContainer.Inject(this);
        }

        protected override void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            UpdateSelectedToDo?.Invoke(ToDoId, !isOn);
            base.OnPointerDown(eventData);
        }

        private void Render()
        {
            if (!ToDos.ContainsKey(ToDoId)) return;
            UpdateView(ToDos[ToDoId]);
        }

        private void UpdateView(ToDo value)
        {
            isOn = value.Selected;
        }
    }
}