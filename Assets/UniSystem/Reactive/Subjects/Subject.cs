using System;
using System.Collections.Generic;
using UniSystem.Reactive.Disposables;

namespace UniSystem.Reactive.Subjects
{
    public static class Subject
    {
        public static ISubject<TSource, TResult> Create<TSource, TResult>(IObserver<TSource> observer,
            IObservable<TResult> observable)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            if (observable == null)
                throw new ArgumentNullException(nameof(observable));
            return new AnonymousSubject<TSource, TResult>(observer, observable);
        }

        private class AnonymousSubject<T, U> : ISubject<T, U>
        {
            private readonly IObserver<T> _observer;
            private readonly IObservable<U> _observable;

            public AnonymousSubject(IObserver<T> observer, IObservable<U> observable)
            {
                _observer = observer;
                _observable = observable;
            }

            public void OnCompleted()
            {
                _observer.OnCompleted();
            }

            public void OnError(Exception error)
            {
                if (error == null)
                    throw new ArgumentNullException(nameof(error));

                _observer.OnError(error);
            }

            public void OnNext(T value)
            {
                _observer.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<U> observer)
            {
                if (observer == null)
                    throw new ArgumentNullException(nameof(observer));

                //
                // [OK] Use of unsafe Subscribe: non-pretentious wrapping of an observable sequence.
                //
                return _observable.Subscribe /*Unsafe*/(observer);
            }
        }
    }

    public class Subject<T> : ISubject<T>
    {
        private readonly object _observerLock = new object();

        private bool _isStopped;
        private bool _isDisposed;
        private Exception _lastError;

        private readonly List<IObserver<T>> _observerList = new List<IObserver<T>>();

        public void OnCompleted()
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                foreach (var observer in _observerList)
                {
                    observer.OnCompleted();
                }
                _isStopped = true;
            }
        }

        public void OnError(Exception error)
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                foreach (var observer in _observerList)
                {
                    observer.OnError(error);
                }
                _isStopped = true;
                _lastError = error;
            }
        }

        public void OnNext(T value)
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped) return;
                foreach (var observer in _observerList)
                {
                    observer.OnNext(value);
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            Exception ex;

            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (!_isStopped)
                {
                    _observerList.Add(observer);
                    return new Subscription(this, observer);
                }

                ex = _lastError;
            }

            if (ex != null)
            {
                observer.OnError(ex);
            }
            else
            {
                observer.OnCompleted();
            }

            return Disposable.Empty;
        }

        public void Dispose()
        {
            lock (_observerLock)
            {
                _isDisposed = true;
                _observerList.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("");
        }

        private class Subscription : IDisposable
        {
            private readonly object _gate = new object();
            private Subject<T> _parent;
            private IObserver<T> _unsubscribeTarget;

            public Subscription(Subject<T> parent, IObserver<T> unsubscribeTarget)
            {
                _parent = parent;
                _unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (_gate)
                {
                    if (_parent == null) return;
                    lock (_parent._observerLock)
                    {
                        _parent._observerList.Remove(_unsubscribeTarget);

                        _unsubscribeTarget = null;
                        _parent = null;
                    }
                }
            }
        }
    }

    public interface ISubject<in TSource, out TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }

    public interface ISubject<T> : ISubject<T, T>
    {
    }
}