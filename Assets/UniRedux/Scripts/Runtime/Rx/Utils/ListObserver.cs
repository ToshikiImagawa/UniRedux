using System;
using System.Linq;
using UniSystem.Collections.Immutable;

namespace UniRedux
{
    internal class ListObserver
    {
        public static ListObserver<T> Empty<T>()
        {
            return new ListObserver<T>(ImmutableList<IObserver<T>>.Empty);
        }
    }

    internal class ListObserver<T> : IObserver<T>
    {
        private readonly ImmutableList<IObserver<T>> _observers;

        public ListObserver(ImmutableList<IObserver<T>> observers)
        {
            _observers = observers;
        }

        public void OnCompleted()
        {
            if (_observers == null || _observers.Count == 0) return;
            foreach (var target in _observers.ToImmutableArray())
            {
                target.OnCompleted();
            }
        }

        public void OnError(Exception error)
        {
            if (_observers == null || _observers.Count == 0) return;
            foreach (var target in _observers.ToImmutableArray())
            {
                target.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            if (_observers == null || _observers.Count == 0) return;
            foreach (var target in _observers.ToImmutableArray())
            {
                target.OnNext(value);
            }
        }

        internal ListObserver<T> Add(IObserver<T> observer)
        {
            return new ListObserver<T>(_observers.Add(observer));
        }

        internal ListObserver<T> Remove(IObserver<T> observer)
        {
            var data = _observers.ToArray();
            var i = Array.IndexOf(data, observer);
            return i < 0 ? this : new ListObserver<T>(_observers.Remove(observer));
        }
    }
}