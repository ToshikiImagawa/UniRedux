using System;
using UniSystem.Collections.Immutable;
using UniSystem.Reactive.Disposables;

namespace UniRedux
{
    public class BehaviorSubject<T> : IObserver<T>, IObservable<T>, IDisposable
    {
        private readonly object _observerLock = new object();
        private bool _isDisposed;
        private bool _isStopped;
        private Exception _lastError;
        private T _lastValue;
        private IObserver<T> _outObserver = EmptyObserver<T>.Instance;

        private T _result;

        public T Value
        {
            get
            {
                ThrowIfDisposed();
                _lastError?.Throw();
                return _lastValue;
            }
        }

        public bool HasObservers => !(_outObserver is EmptyObserver<T>) && !_isStopped && !_isDisposed;

        void IObserver<T>.OnCompleted()
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

        void IObserver<T>.OnError(Exception error)
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

        void IObserver<T>.OnNext(T value)
        {
            IObserver<T> current;
            lock (_observerLock)
            {
                if (_isStopped) return;

                _lastValue = value;
                current = _outObserver;
            }

            current.OnNext(value);
        }

        public void Dispose()
        {
            lock (_observerLock)
            {
                _isDisposed = true;
                _outObserver = new ListObserver<T>(ImmutableList.Create<IObserver<T>>(DisposedObserver<T>.Instance));
                _lastError = null;
                _lastValue = default(T);
            }
        }

        public BehaviorSubject(T defaultValue)
        {
            _lastValue = defaultValue;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) throw new ObjectDisposedException("");
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            var error = default(Exception);
            var value = default(T);
            var subscription = default(Subscription);
            lock (_observerLock)
            {
                ThrowIfDisposed();
                if (_isStopped)
                {
                    error = _lastError;
                }
                else
                {
                    var listObserver = _outObserver as ListObserver<T>;
                    _outObserver = listObserver != null
                        ? listObserver.Add(observer)
                        : new ListObserver<T>(ImmutableList.Create(observer));

                    value = _lastValue;
                    subscription = new Subscription(this, observer);
                }
            }

            if (subscription != null)
            {
                observer.OnNext(value);
                return subscription;
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

        private class Subscription : IDisposable
        {
            private readonly object _disposeLock = new object();
            private BehaviorSubject<T> _parent;
            private IObserver<T> _unsubscribeTarget;

            public Subscription(BehaviorSubject<T> parent, IObserver<T> unsubscribeTarget)
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
                        _parent._outObserver = listObserver != null
                            ? listObserver.Remove(_unsubscribeTarget) as IObserver<T>
                            : EmptyObserver<T>.Instance;
                        _unsubscribeTarget = null;
                        _parent = null;
                    }
                }
            }
        }
    }
}