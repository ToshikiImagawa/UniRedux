using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.UI
{
    public class OptionButton : Button
    {
        [SerializeField] private OptionType _optionType;
        bool _toggle = true;

        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(Run);
        }

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
            
            if(action != null)
            {
                ToDoApplication.CurrentStore.Dispatch(action);
                _toggle = !_toggle;
            }
        }
    }

    public enum OptionType
    {
        SellectAll = 0,
        ToggleCompliteg = 1,
        Remove = 3
    }
}
