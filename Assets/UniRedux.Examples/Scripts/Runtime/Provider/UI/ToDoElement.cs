using System;
using System.Collections.Generic;
using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    public class ToDoElement : MonoBehaviour, IUniReduxComponent
    {
        private IDisposable _disposable;
        private Func<Dictionary<string, object>> _stateProps;

        private int ToDoId { get; set; } = -1;

        private Text _toDoTitle;
        private SelectedToggle _toDoSelector;
        private CompleteButton _completeButton;
        private Dictionary<int, ToDo> _toDos;
        
        [UniReduxInject]
        private Dictionary<int, ToDo> ToDos
        {
            get { return _toDos; }
            set
            {
                _toDos = value;
                Render();
            }
        }

        private Text ToDoTitle => _toDoTitle != null
            ? _toDoTitle
            : _toDoTitle = this.GetComponentFindNameInChildren<Text>("ToDoTitle");

        private SelectedToggle ToDoSelectToggle
            => _toDoSelector != null
                ? _toDoSelector
                : _toDoSelector = this.GetComponentFindNameInChildren<SelectedToggle>("Selector");

        private CompleteButton CompleteButton
            => _completeButton != null
                ? _completeButton
                : _completeButton = this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton");

        private void Render()
        {
            if (!ToDos.ContainsKey(ToDoId)) return;
            UpdateView(ToDos[ToDoId]);
        }

        private void OnDestroy()
        {
            _disposable?.Dispose();
        }

        public void Init(int toDoId)
        {
            if (ToDoId == toDoId) return;
            ToDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggle.Init(toDoId);
            CompleteButton.Init(toDoId);

            Render();
        }

        private void UpdateView(ToDo value)
        {
            var toDoText = value.Text;
            var isCompleted = value.Completed;
            var isSelected = value.Selected;

            ToDoTitle.text = toDoText;
            CompleteButton.targetGraphic.color = isCompleted
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            ToDoTitle.color = isCompleted
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            ToDoSelectToggle.isOn = isSelected;
        }

        public class Factory
        {
            private readonly ToDoElement _prefab;

            public Factory(ToDoElement prefab)
            {
                _prefab = prefab;
            }

            public ToDoElement Create(Transform parent)
            {
                var element = Instantiate(_prefab, parent, false);
                element._disposable = ToDoApp.ToDoViewStateStateContainer.Inject(element);
                return element;
            }
        }
    }
}