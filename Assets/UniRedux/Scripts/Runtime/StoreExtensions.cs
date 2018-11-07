using System;
using System.Threading;

namespace UniRedux
{
    public static class StoreExtensions
    {
        public static IDisposable Subscribe<TState>(this IStore<TState> self, Action<VoidMessage> onNext)
        {
            return self.Subscribe(CreateObserver(onNext));
        }

        public static IDisposable Subscribe<TState>(this IStore<TState> self, Action<VoidMessage> onNext,
            Action<Exception> onError)
        {
            return self.Subscribe(CreateObserver(onNext, onError));
        }

        #region Observer

        private static IObserver<T> CreateObserver<T>(Action<T> onNext)
        {
            return CreateObserver(onNext, ex => { ex.Throw(); }, () => { });
        }

        private static IObserver<T> CreateObserver<T>(Action<T> onNext, Action<Exception> onError)
        {
            return CreateObserver(onNext, onError, () => { });
        }

        private static IObserver<T> CreateObserver<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted)
        {
            return onNext == ((Action<T>) ((T t) => { }))
                ? (IObserver<T>) new EmptyOnNextObserver<T>(onError, onCompleted)
                : new Observer<T>(onNext, onError, onCompleted);
        }

        private class Observer<T> : IObserver<T>
        {
            private readonly Action<T> _onNext;
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            private int _isStopped = 0;

            public Observer(Action<T> onNext, Action<Exception> onError, Action onCompleted)
            {
                _onNext = onNext;
                _onError = onError;
                _onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
                if (_isStopped == 0)
                {
                    _onNext(value);
                }
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onError(error);
                }
            }


            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onCompleted();
                }
            }
        }

        private class EmptyOnNextObserver<T> : IObserver<T>
        {
            private readonly Action<Exception> _onError;
            private readonly Action _onCompleted;

            private int _isStopped = 0;

            public EmptyOnNextObserver(Action<Exception> onError, Action onCompleted)
            {
                _onError = onError;
                _onCompleted = onCompleted;
            }

            public void OnNext(T value)
            {
            }

            public void OnError(Exception error)
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onError(error);
                }
            }

            public void OnCompleted()
            {
                if (Interlocked.Increment(ref _isStopped) == 1)
                {
                    _onCompleted();
                }
            }
        }

        #endregion
    }
}