using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Zenject.Examples.Signal
{
    public class OptionButton : Button
    {
        [SerializeField] private OptionType optionType;
        

        private bool _toggle = true;

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            object action;
            switch (optionType)
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
            UniReduxProvider.Store.Dispatch(action);
            _toggle = !_toggle;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}
