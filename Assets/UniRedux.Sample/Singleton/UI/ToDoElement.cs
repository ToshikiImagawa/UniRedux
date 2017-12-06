using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UniRedux.Sample.Singleton.UI
{
    public abstract class ToDoElement : UIBehaviour, IObserver<ToDo>
    {
        private int _toDoId = -1;

        private IDisposable _disposable;

        public int ToDoId => _toDoId;

        private Text _toDoTitle;
        private Text ToDoTitle => _toDoTitle ?? (_toDoTitle = this.GetComponentFindNameInChildren<Text>("ToDoTitle"));
        private SelectedToggle _toDoSelecter;

        private SelectedToggle ToDoSelectToggle
            => _toDoSelecter ?? (_toDoSelecter = this.GetComponentFindNameInChildren<SelectedToggle>("Selecter"));

        private CompleteButton _completeButton;

        private CompleteButton CompleteButton
            =>
                _completeButton ??
                (_completeButton = this.GetComponentFindNameInChildren<CompleteButton>("CompleteButton"));

        public void Init(int toDoId)
        {
            Dispose();
            _toDoId = toDoId;
            ToDoTitle.text = "??????????";
            ToDoSelectToggle.Init(toDoId);
            CompleteButton.Init(toDoId);
            _disposable = ToDoSelector.FilterToDoElements(toDoId, CurrentStore).Subscribe(this);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
            _toDoId = -1;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            Debug.LogError(error);
        }

        public void OnNext(ToDo value)
        {
            var _toDoText = value.Text;
            var _isCompleted = value.Completed;
            var _isSelected = value.Selected;
            
            ToDoTitle.text = _toDoText;
            CompleteButton.targetGraphic.color = _isCompleted
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            ToDoTitle.color = _isCompleted
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            ToDoSelectToggle.isOn = _isSelected;
            Debug.Log("OnNext");
        }
        protected abstract IStore<ToDoState> CurrentStore { get; }
    }
}
