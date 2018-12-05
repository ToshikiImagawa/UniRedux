using System;
using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    public class CompleteButton : Button, IUniReduxComponent
    {
        private int _toDoId = -1;
        private IDisposable _disposable;

        [UniReduxInject(PropertyName = "ToggleCompletedToDo")]
        private Action<int> ToggleCompleted { get; set; }

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ToggleCompleted?.Invoke(_toDoId);
            base.OnPointerDown(eventData);
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
    }
}