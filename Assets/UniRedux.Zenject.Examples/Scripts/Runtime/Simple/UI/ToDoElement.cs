using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;

namespace UniRedux.Zenject.Examples.Simple
{
    public class ToDoElement : UIBehaviour
    {
        private IStore<ToDoState> _store;

        private IDisposable _disposable;

        private ToDo _toDo = new ToDo();

        public int ToDoId { get; private set; } = -1;

        private Text ToDoTitle
            => this.GetComponentFindNameInChildren<Text>("ToDoTitle");

        private SelectedToggle ToDoSelectToggle
            => this.GetComponentFindNameInChildren<SelectedToggle>("Selector");

        private CompleteButton CompleteButton
            => this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton");

        [Inject]
        private void Injection(IStore<ToDoState> store)
        {
            _store = store;
            _disposable = _store.Subscribe(state => { HandleChange(); });
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

            var toDo = _store?.GetState().GetFilterToDos().FirstOrDefault(todo => todo.Id == ToDoId);
            if (toDo == null) return;
            UpdateView(toDo);
        }

        private void HandleChange()
        {
            var toDo = _store.GetState().GetFilterToDos().FirstOrDefault(todo => todo.Id == ToDoId);
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
            [Inject] private Transform _parentTransform;

            protected override void OnCreated(ToDoElement item)
            {
                item.transform.SetParent(_parentTransform, false);
                item.gameObject.SetActive(false);
                item.name = "ToDoElement";
            }

            protected override void OnSpawned(ToDoElement item)
            {
                item.gameObject.SetActive(true);
            }

            protected override void OnDespawned(ToDoElement item)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}