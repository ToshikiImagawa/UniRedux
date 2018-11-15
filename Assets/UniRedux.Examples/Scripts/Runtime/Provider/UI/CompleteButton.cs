using System;
using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    public class CompleteButton : Button, IUniReduxComponent
    {
        private int _toDoId = -1;
        
        [UniReduxInject]
        private Action<int> ToggleCompletedToDo { get; set; }

        public void Init(int toDoId)
        {
            _toDoId = toDoId;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            ToggleCompletedToDo?.Invoke(_toDoId);
            base.OnPointerDown(eventData);
        }

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            ToDoApp.ToDoViewStateStateContainer.Inject(this);
        }
    }
}