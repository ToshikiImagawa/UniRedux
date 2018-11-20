using System;
using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class OptionButton : Button, IUniReduxComponent
    {
        [SerializeField] private OptionType optionType;

        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            container.Inject(this);
        }

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
            Dispatch(action);
            _toggle = !_toggle;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}