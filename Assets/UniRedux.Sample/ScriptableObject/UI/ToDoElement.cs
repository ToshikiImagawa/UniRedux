using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UniRedux.Sample.ScriptableObject.UI
{
    public class ToDoElement : UIBehaviour, IObserver<ToDo>
    {
        [SerializeField] private UnityEngine.ScriptableObject _toDoApplicationObject;
        private Application<ToDoState> ToDoApplication => _toDoApplicationObject as Application<ToDoState>;
        private int _toDoId = -1;
        private string _toDoText = string.Empty;
        private bool _isCompleted;
        private bool _isSelected;

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
            _disposable = ToDoSelector.FilterToDoElements(toDoId, ToDoApplication?.CurrentStore).Subscribe(this);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
            _disposable = null;
            _toDoId = -1;
        }

        public void OnCompleted()
        {
            ToDoTitle.text = _toDoText;
            CompleteButton.targetGraphic.color = _isCompleted
                ? new Color(233f / 255f, 147f / 255f, 40f / 255f)
                : new Color(137f / 255f, 137f / 255f, 137f / 255f);
            ToDoTitle.color = _isCompleted
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
            ToDoSelectToggle.isOn = _isSelected;
        }

        public void OnError(Exception error)
        {
            Debug.LogError(error);
        }

        public void OnNext(ToDo value)
        {
            _toDoText = value.Text;
            _isCompleted = value.Completed;
            _isSelected = value.Selected;
        }
    }
}
