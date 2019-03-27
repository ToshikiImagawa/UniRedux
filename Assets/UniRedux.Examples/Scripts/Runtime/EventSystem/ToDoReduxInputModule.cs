using System.Collections.Generic;
using UniRedux.EventSystems;

namespace UniRedux.Examples.EventSystem
{
    public class ToDoReduxInputModule : StateCheckerInputModule<ToDoState>
    {
        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;

        protected override void BindingPoint()
        {
            Bind<IToDoListEventSystemHandler, IDictionary<int, ToDo>>(
                (handler, eventData) => { handler.OnChangeToDoList(); },
                state => state.ToDos
            );
            Bind<IToDoFilterEventSystemHandler, ToDoFilter>(
                (handler, eventData) => { handler.OnChangeToDoFilter(); },
                state => state.Filter
            );
        }

        protected override void Init()
        {
            StartMonitoring();
        }
    }
}