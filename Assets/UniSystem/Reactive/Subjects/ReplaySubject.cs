using System;
using System.Collections.Generic;
using UniSystem.Reactive.Disposables;

namespace UniSystem.Reactive.Subjects
{
    public class ReplaySubject<T> : ISubject<T>, IDisposable
    {
        private readonly object _observerLock = new object();

        private bool isStopped;
        private bool isDisposed;
        private Exception lastError;

        private readonly List<IObserver<T>> _observerList = new List<IObserver<T>>();

        private T _queue; // todo: Queue<TimeInterval<T>> queue = new Queue<TimeInterval<T>>();

        public void OnCompleted()
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                foreach (var observer in _observerList)
                {
                    observer.OnCompleted();
                }
            }
        }

        public void OnError(Exception error)
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                foreach (var observer in _observerList)
                {
                    observer.OnError(error);
                }
                isStopped = true;
                lastError = error;
            }
        }

        public void OnNext(T value)
        {
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (isStopped) return;

                foreach (var observer in _observerList)
                {
                    observer.OnNext(value);
                    _queue = value;
                }
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            Exception ex;
            var subscription = default(Subscription);

            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (!isStopped)
                {
                    _observerList.Add(observer);
                    subscription = new Subscription(this, observer);
                }
                ex = lastError;
                if (_queue != null)
                {
                    observer.OnNext(_queue);
                }
            }

            if (subscription != null)
            {
                return subscription;
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
                isDisposed = true;
                _observerList.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed) throw new ObjectDisposedException("");
        }

        private class Subscription : IDisposable
        {
            private readonly object _gate = new object();
            private ReplaySubject<T> _parent;
            private IObserver<T> _unsubscribeTarget;

            public Subscription(ReplaySubject<T> parent, IObserver<T> unsubscribeTarget)
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
}