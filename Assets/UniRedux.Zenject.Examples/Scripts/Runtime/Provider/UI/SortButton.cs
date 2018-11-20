using UniRedux.Provider;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Provider
{
    public class SortButton : Button, IUniReduxComponent
    {
        private ToDoFilter _filter;
        [SerializeField] private ToDoFilter filterType;

        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }

        [Inject]
        private void Injection([Inject(Id = "ToDoViewContainer")] IUniReduxContainer container)
        {
            container.Inject(this);
        }


        [UniReduxInject]
        private ToDoFilter Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                Render();
            }
        }

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            Dispatch(ToDoActionCreator.ChangeToDoFilter(filterType));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        private void Render()
        {
            interactable = Filter != filterType;
        }
    }
}