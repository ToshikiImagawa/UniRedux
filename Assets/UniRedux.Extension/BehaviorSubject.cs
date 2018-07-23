using System;
using System.Collections.Generic;

namespace UniRedux
{
    public interface IBehaviorSubject<in TSource, out TResult> : IObserver<TSource>, IObservable<TResult>
    {
    }

    public static class BehaviorSubject
    {
        public static IBehaviorSubject<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> converter)
            where TResult : IEqualityComparer<TResult>, new()
        {
            return new BehaviorSubject<TSource, TResult>(converter, new TResult());
        }

        public static IBehaviorSubject<TSource, TResult> Create<TSource, TResult>(Func<TSource, TResult> converter,
            IEqualityComparer<TResult> changeChecker)
        {
            return new BehaviorSubject<TSource, TResult>(converter, changeChecker);
        }
    }

    internal class BehaviorSubject<TSource, TResult> : IBehaviorSubject<TSource, TResult>
    {
        private event Action CompletedAction;
        private event Action<Exception> ErrorAction;
        private event Action<TResult> NextAction;

        private Func<TSource, TResult> Converter { get; }

        private IEqualityComparer<TResult> ChangeChecker { get; }

        private TResult _result;

        void IObserver<TSource>.OnCompleted()
        {
            CompletedAction?.Invoke();
        }

        void IObserver<TSource>.OnError(Exception error)
        {
            ErrorAction?.Invoke(error);
        }

        void IObserver<TSource>.OnNext(TSource value)
        {
            var result = Converter(value);
            if (ChangeChecker.Equals(_result, result)) return;
            NextAction?.Invoke(result);
            _result = result;
        }

        internal BehaviorSubject(Func<TSource, TResult> converter, IEqualityComparer<TResult> changeChecker)
        {
            if (converter == null) throw new ArgumentNullException(nameof(converter), "Cannot set property of null.");
            if (changeChecker == null)
                throw new ArgumentNullException(nameof(changeChecker), "Cannot set property of null.");
            Converter = converter;
            ChangeChecker = changeChecker;
        }

        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            CompletedAction -= observer.OnCompleted;
            ErrorAction -= observer.OnError;
            NextAction -= observer.OnNext;

            if (_result != null)
            {
                observer.OnNext(_result);
            }

            CompletedAction += observer.OnCompleted;
            ErrorAction += observer.OnError;
            NextAction += observer.OnNext;

            return new Disposer(() =>
            {
                CompletedAction -= observer.OnCompleted;
                ErrorAction -= observer.OnError;
                NextAction -= observer.OnNext;
            });
        }

        private sealed class Disposer : IDisposable
        {
            private readonly Action _disposeListener;
            private bool _disposedValue = false;

            public Disposer(Action disposeListener)
            {
                _disposeListener = disposeListener;
            }

            private void Dispose(bool disposing)
            {
                if (_disposedValue) return;
                if (disposing)
                {
                    _disposeListener?.Invoke();
                }

                _disposedValue = true;
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
            }
        }
    }
}