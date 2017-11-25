using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Sample
{
    public static class ToDoSelector
    {
        private static FilterToDoSubject _filterToDoSelector;

        private static FilterToDoSubject FilterToDoSelector(IStore<ToDoState> toDoStore)
        {
            if (_filterToDoSelector != null) return _filterToDoSelector;
            _filterToDoSelector = new FilterToDoSubject();
            toDoStore?.Subscribe(_filterToDoSelector);
            return _filterToDoSelector;
        }

        public static IObservable<ToDo[]> FilterToDos(IStore<ToDoState> toDoStore) => FilterToDoSelector(toDoStore);

        public static IObservable<ToDo> FilterToDoElements(int toDoId, IStore<ToDoState> toDoScriptableStore)
            => FilterToDoElementSelector(toDoId,toDoScriptableStore);

        private static readonly IDictionary<int, FilterToDoElementSubject> FilterToDoElementSelectorDictionary =
            new Dictionary<int, FilterToDoElementSubject>();

        private static FilterToDoElementSubject FilterToDoElementSelector(int toDoId, IStore<ToDoState> toDoStore)
        {
            FilterToDoElementSubject filterToDoElementSelector;
            if (FilterToDoElementSelectorDictionary.TryGetValue(toDoId, out filterToDoElementSelector))
                return filterToDoElementSelector;
            filterToDoElementSelector = new FilterToDoElementSubject(toDoId);
            FilterToDoElementSelectorDictionary[toDoId] = filterToDoElementSelector;
            FilterToDoSelector(toDoStore).Subscribe(filterToDoElementSelector);
            return filterToDoElementSelector;
        }

        private class FilterToDoSubject : IObserver<ToDoState>, IObservable<ToDo[]>
        {
            private event Action CompletedAction;
            private event Action<Exception> ErrorAction;
            private event Action<ToDo[]> NextAction;
            private ToDo[] _toDos;

            void IObserver<ToDoState>.OnCompleted()
            {
                CompletedAction?.Invoke();
            }

            void IObserver<ToDoState>.OnError(Exception error)
            {
                ErrorAction?.Invoke(error);
            }

            void IObserver<ToDoState>.OnNext(ToDoState value)
            {
                var toDos = value.ToDos.Where(toDo =>
                {
                    switch (value.Filter)
                    {
                        case TodosFilter.All:
                            return true;
                        case TodosFilter.Completed:
                            return toDo.Completed;
                        case TodosFilter.InProgress:
                            return !toDo.Completed;
                        default:
                            return false;
                    }
                }).ToArray();
                NextAction?.Invoke(toDos);
                _toDos = toDos;
            }

            public IDisposable Subscribe(IObserver<ToDo[]> observer)
            {
                CompletedAction -= observer.OnCompleted;
                NextAction -= observer.OnNext;
                ErrorAction -= observer.OnError;

                CompletedAction += observer.OnCompleted;
                NextAction += observer.OnNext;
                ErrorAction += observer.OnError;

                if (_toDos != null)
                {
                    observer.OnNext(_toDos);
                    observer.OnCompleted();
                }

                return Util.CreateDisposer(() =>
                {
                    CompletedAction -= observer.OnCompleted;
                    NextAction -= observer.OnNext;
                    ErrorAction -= observer.OnError;
                });
            }
        }

        private class FilterToDoElementSubject : IObserver<ToDo[]>, IObservable<ToDo>
        {
            private readonly int _toDoId;
            private ToDo _toDo;

            private event Action CompletedAction;
            private event Action<Exception> ErrorAction;
            private event Action<ToDo> NextAction;

            public FilterToDoElementSubject(int toDoId)
            {
                _toDoId = toDoId;
            }

            public void OnNext(ToDo[] value)
            {
                foreach (var toDo in value)
                {
                    if (toDo.Id == _toDoId)
                    {
                        NextAction?.Invoke(toDo);
                        _toDo = toDo;
                    }
                }
            }

            public void OnError(Exception error)
            {
                ErrorAction?.Invoke(error);
            }

            public void OnCompleted()
            {
                CompletedAction?.Invoke();
            }

            public IDisposable Subscribe(IObserver<ToDo> observer)
            {
                CompletedAction -= observer.OnCompleted;
                NextAction -= observer.OnNext;
                ErrorAction -= observer.OnError;

                CompletedAction += observer.OnCompleted;
                NextAction += observer.OnNext;
                ErrorAction += observer.OnError;

                if (_toDo != null)
                {
                    observer.OnNext(_toDo);
                    observer.OnCompleted();
                }

                return Util.CreateDisposer(() =>
                {
                    CompletedAction -= observer.OnCompleted;
                    NextAction -= observer.OnNext;
                    ErrorAction -= observer.OnError;
                });
            }
        }
    }
}
