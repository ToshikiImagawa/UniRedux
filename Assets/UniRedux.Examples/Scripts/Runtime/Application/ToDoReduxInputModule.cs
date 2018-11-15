using System.Collections.Generic;
using UniRedux.EventSystems;

namespace UniRedux.Examples.Application
{
    public class ToDoReduxInputModule : StateCheckerInputModule<ToDoState>
    {
        protected override IStore<ToDoState> CurrentStore
            => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;

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