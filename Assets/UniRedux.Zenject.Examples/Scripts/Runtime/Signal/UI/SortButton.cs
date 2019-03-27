using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class SortButton : Button
    {
        private ToDoFilter _filter;
        [SerializeField] private ToDoFilter filterType;
        [Inject]
        private UniReduxSignalBus uniReduxSignalBus;

        private void Run()
        {
            DispatchAction();
        }

        private void DispatchAction()
        {
            UniReduxProvider.Store.Dispatch(ToDoActionCreator.ChangeToDoFilter(filterType));
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            Run();
            base.OnPointerDown(eventData);
        }

        protected override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying) return;

            uniReduxSignalBus?.Subscribe<ToDoFilter>(OnChange);
            UpdateDisplay();
        }

        private void OnChange()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            interactable = UniReduxProvider.GetStore<ToDoState>().GetState().Filter != filterType;
        }
    }
}
