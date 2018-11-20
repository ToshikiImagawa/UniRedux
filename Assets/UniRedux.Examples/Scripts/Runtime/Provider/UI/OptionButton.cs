using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace UniRedux.Provider.Examples
{
    public class OptionButton : Button, IUniReduxComponent
    {
        [SerializeField] private OptionType _optionType;
        private bool _toggle = true;
        private IDisposable _disposable;

        [UniReduxInject]
        private Action RemoveSelectedTodo { get; set; }

        [UniReduxInject]
        private Action<bool> UpdateSelectedAllToDo { get; set; }

        [UniReduxInject]
        private Action<bool> CompleteSelectedTodo { get; set; }

        private void Run()
        {
            switch (_optionType)
            {
                case OptionType.Remove:
                    RemoveSelectedTodo?.Invoke();
                    break;
                case OptionType.SelectAll:
                    UpdateSelectedAllToDo?.Invoke(_toggle);
                    break;
                case OptionType.ToggleCompleted:
                    CompleteSelectedTodo?.Invoke(_toggle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _toggle = !_toggle;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        protected override void Awake()
        {
            _disposable = ToDoApp.ToDoViewStateStateContainer.Inject(this);
        }

        protected override void OnDestroy()
        {
            _disposable?.Dispose();
        }
    }
}