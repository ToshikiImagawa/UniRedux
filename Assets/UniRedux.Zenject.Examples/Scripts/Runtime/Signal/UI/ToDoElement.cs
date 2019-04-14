using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class ToDoElement : MonoBehaviour
    {
        [Inject] private UniReduxSignalBus _reduxSignalBus;
        private ToDo _toDo = new ToDo();

        public int ToDoId { get; private set; } = -1;

        private CompleteButton CompleteButton => this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton");
        private Text Title => this.GetComponentFindNameInChildren<Text>("ToDoTitle");
        private SelectedToggle Selector => this.GetComponentFindNameInChildren<SelectedToggle>("Selector");

        public void SetId(int id)
        {
            ToDoId = id;
            OnChangeState(UniReduxProvider.GetStore<ToDoState>().GetState().ToDos
                .FirstOrDefault(todo => todo.Id == ToDoId));
        }

        private void OnChangeState(ToDo toDo)
        {
            if (toDo == null) return;
            if (_toDo.Equals(toDo)) return;
            _toDo = toDo;
            CompleteButton.targetGraphic.color = _toDo.Completed
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            Title.text = _toDo.Text;
            Title.color = _toDo.Completed
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            Selector.isOn = _toDo.Selected;
        }

        public class Pool : MemoryPool<ToDoElement>
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
                item._reduxSignalBus.Subscribe<ToDo>(item.OnChangeState);
            }

            protected override void OnDespawned(ToDoElement item)
            {
                item.gameObject.SetActive(false);
                item._reduxSignalBus.Unsubscribe<ToDo>(item.OnChangeState);
            }
        }
    }
}