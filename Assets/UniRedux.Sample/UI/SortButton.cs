using UnityEngine;
using UnityEngine.UI;

namespace UniRedux.Sample.UI
{
    public class SortButton : Button
    {
        [SerializeField] private TodosFilter _filterType;

        protected override void Awake()
        {
            base.Awake();

            onClick.AddListener(Run);
        }

        private void Run()
        {
            DispachAction();
        }


        private void DispachAction()
        {
            ToDoApplication.CurrentStore.Dispatch(new ChangeToDosFilterAction
            {
                Filter = _filterType
            });
        }
    }
}