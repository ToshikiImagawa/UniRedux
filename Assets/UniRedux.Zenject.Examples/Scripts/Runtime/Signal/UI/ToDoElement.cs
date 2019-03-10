using System.Linq;
using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class ToDoElement : MonoBehaviour
    {
        [Inject]
        private UniReduxSignalBus uniReduxSignalBus;
        private ToDo _hitToDo = new ToDo();

        public int ToDoId { get; private set; } = -1;
        private Text ToDoTitle => this.GetComponentFindNameInChildren<Text>("ToDoTitle");

        private SelectedToggle ToDoSelectToggle => this.GetComponentFindNameInChildren<SelectedToggle>("Selector");

        private CompleteButton CompleteButton => this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton");

        public void SetId(int id)
        {
            ToDoId = id;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            var hitTodo = UniReduxProvider.GetStore<ToDoState>().GetState().ToDos.FirstOrDefault(todo => todo.Id == ToDoId);
            if (hitTodo == null) throw Assert.CreateException();
            if (_hitToDo.Equals(hitTodo)) return;
            _hitToDo = hitTodo;
            ToDoTitle.text = _hitToDo.Text;
            CompleteButton.targetGraphic.color = _hitToDo.Completed
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            ToDoTitle.color = _hitToDo.Completed
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            ToDoSelectToggle.isOn = _hitToDo.Selected;
        }

        private void OnChangeState()
        {
            UpdateDisplay();
        }

        public class Pool : MemoryPool<Transform, ToDoElement>
        {
            protected override void Reinitialize(Transform parentTransform, ToDoElement element)
            {
                element.transform.SetParent(parentTransform, false);
            }

            protected override void OnCreated(ToDoElement item)
            {
                item.uniReduxSignalBus.Subscribe<ToDo[], ToDoState>(item.OnChangeState);
            }

            protected override void OnSpawned(ToDoElement item)
            {
                base.OnSpawned(item);
            }
        }
    }
}
