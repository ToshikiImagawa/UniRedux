using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.UI
{
    public class OptionButton : Button
    {
        [SerializeField] private UnityEngine.ScriptableObject _toDoApplicationObject;
        [SerializeField] private OptionType _optionType;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
        bool _toggle = true;

        private void Run()
        {
            DispachAction();
        }

        private void DispachAction()
        {
            object action = null;
            switch (_optionType)
            {
                case OptionType.Remove:
                    action = new RemoveSelectedTodosAction();
                    break;
                case OptionType.SellectAll:
                    action = new UpdateSelectedAllToDoAction
                    {
                        IsSelected = _toggle
                    };
                    break;
                case OptionType.ToggleCompliteg:
                    action = new CompleteSelectedTodosAction
                    {
                        IsCompleted = _toggle
                    };
                    break;
            }

            if (action == null) return;
            ToDoApplication?.CurrentStore?.Dispatch(action);
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
        SellectAll = 0,
        ToggleCompliteg = 1,
        Remove = 3
    }
}
