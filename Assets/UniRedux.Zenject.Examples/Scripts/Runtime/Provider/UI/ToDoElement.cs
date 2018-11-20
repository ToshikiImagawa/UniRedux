using System;
using System.Linq;
using UniRedux.Provider;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class ToDoElement : UIBehaviour, IUniReduxComponent
    {
        private IDisposable _disposable;

        private ToDo _toDo = new ToDo();
        private ToDo[] _toDos;

        public int ToDoId { get; private set; } = -1;
        private Text ToDoTitle => this.GetComponentFindNameInChildren<Text>("ToDoTitle");

        private SelectedToggle ToDoSelectToggle => this.GetComponentFindNameInChildren<SelectedToggle>("Selector");

        private CompleteButton CompleteButton => this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton");

        [UniReduxInject]
        private ToDo[] FilterToDos
        {
            get { return _toDos; }
            set
            {
                _toDos = value;
                Render();
            }
        }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            _disposable = container.Inject(this);
        }

        protected override void OnDestroy()
        {
            _disposable.Dispose();
        }

        public void Init(int toDoId)
        {
            ToDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggle.Init(toDoId);
            CompleteButton.Init(toDoId);

            var toDo = _toDos.FirstOrDefault(todo => todo.Id == ToDoId);
            if (toDo == null) return;
            UpdateView(toDo);
        }

        private void Render()
        {
            var toDo = _toDos.FirstOrDefault(todo => todo.Id == ToDoId);
            if (toDo == null) return;
            if (_toDo.Id == toDo.Id &&
                _toDo.Completed == toDo.Completed &&
                _toDo.Selected == toDo.Selected &&
                _toDo.Text == toDo.Text) return;
            _toDo = toDo;
            UpdateView(toDo);
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

        public class Pool : MonoMemoryPool<Transform, ToDoElement>
        {
            protected override void Reinitialize(Transform parentTransform, ToDoElement element)
            {
                element.transform.SetParent(parentTransform, false);
            }
        }
    }
}