using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UniRedux.Provider.Examples
{
    public class AddToDoButton : Button, IUniReduxComponent
    {
        [SerializeField] private InputField _toDoMessageInputField;

        private IToDoMessageInputModule _toDoMessageInputModule;

        private IToDoMessageInputModule ToDoMessageInputModule =>
            _toDoMessageInputModule ??
            (_toDoMessageInputModule = _toDoMessageInputField.GetComponent<IToDoMessageInputModule>());

        [UniReduxInject]
        private Action<string> AddToDo { get; set; }

        private void Run()
        {
            if (ToDoMessageInputModule == null) return;
            var text = ToDoMessageInputModule.ToDoMessage;

            if (string.IsNullOrEmpty(text)) return;
            AddToDo?.Invoke(text);
            ToDoMessageInputModule.ToDoMessage = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
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