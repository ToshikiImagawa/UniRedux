using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal
{
    public class ToDoTitle : MonoBehaviour
    {
        [Inject]
        private UniReduxSignalBus _uniReduxSignalBus;
        private Text _title;
        private Text Title => _title != null ? _title : _title = GetComponent<Text>();

        private void Awake()
        {
            _uniReduxSignalBus.Subscribe<ToDo>(OnChangeState);
        }

        private void OnChangeState(ToDo toDo)
        {
            if (toDo == null) return;
            Title.text = toDo.Text;
            Title.color = toDo.Completed
                ? new Color(170f / 170f, 147f / 255f, 170f / 255f)
                : new Color(50f / 255f, 50f / 255f, 50f / 255f);
        }
    }
}
