using System;
using System.Threading;

namespace UniRedux.Rx
{
    public static class Observer
    {
        internal static IObserver<T> CreateSubscribeObserver<T>(Action<T> onNext, Action<Exception> onError,
            Action onCompleted)
        {
            if (onNext == Stubs<T>.Ignore) return new EmptySubscribe<T>(onError, onCompleted);
            return new Subscribe<T>(onNext, onError, onCompleted);
        }
    }

    internal class EmptySubscribe<T> : IObserver<T>
    {
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        private int _isStopped = 0;

        public EmptySubscribe(Action<Exception> onError, Action onCompleted)
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

    internal class Subscribe<T> : IObserver<T>
    {
        private readonly Action<T> _onNext;
        private readonly Action<Exception> _onError;
        private readonly Action _onCompleted;

        private int _isStopped = 0;

        public Subscribe(Action<T> onNext, Action<Exception> onError, Action onCompleted)
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

    internal static class Stubs
    {
        public static readonly Action Nop = () => { };
        public static readonly Action<Exception> Throw = ex => { ex.Throw(); };

        // marker for CatchIgnore and Catch avoid iOS AOT problem.
        public static IObservable<TSource> CatchIgnore<TSource>(Exception ex)
        {
            return Observable.Empty<TSource>();
        }
    }

    internal static class Stubs<T>
    {
        public static readonly Action<T> Ignore = (T t) => { };
        public static readonly Func<T, T> Identity = (T t) => t;
        public static readonly Action<Exception, T> Throw = (ex, _) => { ex.Throw(); };
    }

    internal static class Stubs<T1, T2>
    {
        public static readonly Action<T1, T2> Ignore = (x, y) => { };
        public static readonly Action<Exception, T1, T2> Throw = (ex, _, __) => { ex.Throw(); };
    }

    internal static class Stubs<T1, T2, T3>
    {
        public static readonly Action<T1, T2, T3> Ignore = (x, y, z) => { };
        public static readonly Action<Exception, T1, T2, T3> Throw = (ex, _, __, ___) => { ex.Throw(); };
    }
}