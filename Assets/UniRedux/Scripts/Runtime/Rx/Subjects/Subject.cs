using System;
using UniSystem.Collections.Immutable;
using UniSystem.Reactive.Disposables;

namespace UniRedux
{
    public sealed class Subject<T> : ISubject<T>, IDisposable
    {
        private readonly object _observerLock = new object();

        private bool _isStopped;
        private bool _isDisposed;
        private Exception _lastError;
        private IObserver<T> _outObserver = EmptyObserver<T>.Instance;

        public bool HasObservers => !(_outObserver is EmptyObserver<T>) && !_isStopped && !_isDisposed;

        public void OnCompleted()
        {
            IObserver<T> old;
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped) return;

                old = _outObserver;
                _outObserver = ListObserver.Empty<T>();
                _isStopped = true;
            }

            old.OnCompleted();
        }

        public void OnError(Exception error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            IObserver<T> old;
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped) return;

                old = _outObserver;
                _outObserver = ListObserver.Empty<T>();
                _isStopped = true;
                _lastError = error;
            }

            old.OnError(error);
        }

        public void OnNext(T value)
        {
            _outObserver.OnNext(value);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            Exception error;
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (!_isStopped)
                {
                    var listObserver = _outObserver as ListObserver<T>;
                    if (listObserver != null)
                    {
                        _outObserver = listObserver.Add(observer);
                    }
                    else
                    {
                        var current = _outObserver;
                        if (current is EmptyObserver<T>)
                        {
                            _outObserver = observer;
                        }
                        else
                        {
                            _outObserver = new ListObserver<T>(ImmutableList.Create(new[] {current, observer}));
                        }
                    }

                    return new Subscription(this, observer);
                }

                error = _lastError;
            }

            if (error != null)
            {
                observer.OnError(error);
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
                _outObserver = new ListObserver<T>(ImmutableList.Create<IObserver<T>>(DisposedObserver<T>.Instance));
                _lastError = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("");
        }

        private class Subscription : IDisposable
        {
            private readonly object _disposeLock = new object();
            private Subject<T> _parent;
            private IObserver<T> _unsubscribeTarget;

            public Subscription(Subject<T> parent, IObserver<T> unsubscribeTarget)
            {
                _parent = parent;
                _unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (_disposeLock)
                {
                    if (_parent == null) return;
                    lock (_parent._observerLock)
                    {
                        var listObserver = _parent._outObserver as ListObserver<T>;
                        if (listObserver != null)
                        {
                            _parent._outObserver = listObserver.Remove(_unsubscribeTarget);
                        }
                        else
                        {
                            _parent._outObserver = EmptyObserver<T>.Instance;
                        }

                        _unsubscribeTarget = null;
                        _parent = null;
                    }
                }
            }
        }
    }
}