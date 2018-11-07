using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples
{
    public class OptionButton : Button
    {
        [SerializeField] private OptionType _optionType;
        [Inject]
        private IStore<ToDoState> _store;
        private bool _toggle = true;

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            if (_store == null) return;
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
            _store.Dispatch(action);
            _toggle = !_toggle;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }

    public enum OptionType
    {
        SelectAll = 0,
        ToggleCompleted = 1,
        Remove = 3
    }
}