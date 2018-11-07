using System.Collections.Generic;
using UniRedux.EventSystems;

namespace UniRedux.Examples
{
    public class ToDoReduxInputModule : StateCheckerInputModule<ToDoState>
    {
        protected override IStore<ToDoState> CurrentStore
            => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;

        protected override void BindingPoint()
        {
            Bind<IToDoListEventSystemHandler, IDictionary<int, ToDo>>(
                (handler, eventData) => { handler.OnChangeToDoList(); },
                state => state.ToDos, new ToDoListEqualityComparer()
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

    public interface IToDoListEventSystemHandler : IReduxEventSystemHandler
    {
        void OnChangeToDoList();
    }

    public interface IToDoFilterEventSystemHandler : IReduxEventSystemHandler
    {
        void OnChangeToDoFilter();
    }

    public class ToDoListEqualityComparer : IEqualityComparer<IDictionary<int, ToDo>>
    {
        bool IEqualityComparer<IDictionary<int, ToDo>>.Equals(IDictionary<int, ToDo> x, IDictionary<int, ToDo> y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            if (x.Count != y.Count) return false;

            return x.GetHashCode() == y.GetHashCode();
        }

        int IEqualityComparer<IDictionary<int, ToDo>>.GetHashCode(IDictionary<int, ToDo> obj)
        {
            return obj.GetHashCode();
        }
    }
}