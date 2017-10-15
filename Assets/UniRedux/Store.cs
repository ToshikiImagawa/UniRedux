using System;

namespace UniRedux
{
    public class Store<TState> : IStore<TState>
    {
        private readonly object _syncRoot = new object();
        private readonly Dispatcher _dispatcher;
        private readonly Reducer<TState> _reducer;
        private TState _lastState;
        private event Action _completedListener;
        private event Action<Exception> _errorListener;
        private event Action<TState> _nextListener;

        public object Dispatch(object action)
        {
            return _dispatcher(action);
        }

        public TState GetState() => _lastState;

        public IDisposable Subscribe(IObserver<TState> observer)
        {
            _completedListener += observer.OnCompleted;
            _errorListener += observer.OnError;
            _nextListener += observer.OnNext;
            return new Disposer(() =>
            {
                _completedListener -= observer.OnCompleted;
                _errorListener -= observer.OnError;
                _nextListener -= observer.OnNext;
            });
            throw new NotImplementedException();
        }

        public Store(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] middlewares)
        {
            _reducer = reducer;
            _dispatcher = ApplyMiddlewares(middlewares);

            _lastState = initialState;
        }

        private Dispatcher ApplyMiddlewares(params Middleware<TState>[] middlewares)
        {
            Dispatcher dispatcher = InnerDispatch;
            foreach (var middleware in middlewares)
            {
                dispatcher = middleware(this)(dispatcher);
            }
            return dispatcher;
        }

        private object InnerDispatch(object action)
        {
            lock (_syncRoot)
            {
                _lastState = _reducer(_lastState, action);
            }
            try
            {
                _nextListener?.Invoke(_lastState);
                _completedListener?.Invoke();
            }
            catch (Exception e)
            {
                _errorListener?.Invoke(e);
            }
            return action;
        }

        private class Disposer : IDisposable
        {
            private Action _disposeListener;
            public Disposer(Action disposeListener)
            {
                _disposeListener = disposeListener;
            }

            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _disposeListener?.Invoke();
                    }

                    disposedValue = true;
                }
            }
            void IDisposable.Dispose()
            {
                Dispose(true);
            }
        }
    }

    public interface IStore<TState> : IObservable<TState>
    {
        object Dispatch(object action);
        TState GetState();
    }
}