using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class AddToDoButton : Button
    {
#if UNITY_EDITOR
        [SerializeField] private GameObject _toDoMessageInputModuleObject;
#endif
        [SerializeField, HideInInspector] private Component _toDoMessageInputModule;

        private IToDoMessageInputModule ToDoMessageInputModule => _toDoMessageInputModule as IToDoMessageInputModule;

        private void Run()
        {
            if (ToDoMessageInputModule == null) return;
            var text = ToDoMessageInputModule.ToDoMessage;

            if (!string.IsNullOrEmpty(text))
            {
                DispatchAction(text);
            }
        }

        private void DispatchAction(string text)
        {
            CurrentStore?.Dispatch(ToDoActionCreator.AddToDo(text));
            ToDoMessageInputModule.ToDoMessage = string.Empty;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        private IStore<ToDoState> CurrentStore => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }

    public interface IToDoMessageInputModule
    {
        string ToDoMessage { get; set; }
    }
}