using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class OptionButton : Button
    {
        [SerializeField] private OptionType _optionType;
        private bool _toggle = true;

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            object action;
            switch (_optionType)
            {
                case OptionType.Remove:
                    action = ToDoActionCreator.RemoveSelectedTodo();
                    break;
                case OptionType.SelectAll:
                    action = ToDoActionCreator.UpdateSelectedAllToDo(_toggle);
                    break;
                case OptionType.ToggleCompleted:
                    action = ToDoActionCreator.CompleteSelectedTodo(_toggle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (action == null) return;
            CurrentStore?.Dispatch(action);
            _toggle = !_toggle;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        private IStore<ToDoState> CurrentStore => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }

    public enum OptionType
    {
        SelectAll = 0,
        ToggleCompleted = 1,
        Remove = 3
    }
}