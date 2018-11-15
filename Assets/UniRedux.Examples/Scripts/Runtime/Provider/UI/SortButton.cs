using UniRedux.Examples;
using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Provider.Examples
{
    public class SortButton : Button, IUniReduxComponent
    {
        [SerializeField] private ToDoFilter filterType;
        
        private ToDoFilter _filter;

        [UniReduxInject]
        private Dispatcher Dispatch { get; set; }
        
        
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

        protected override void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            ToDoApp.ToDoViewStateStateContainer.Inject(this);
        }

        private void Run()
        {
            Dispatch?.Invoke(ToDoActionCreator.ChangeToDoFilter(filterType));
        }

        private void Render()
        {
            interactable = Filter != filterType;
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }
    }
}