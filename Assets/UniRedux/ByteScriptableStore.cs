using System;
using UnityEngine;

namespace UniRedux
{
    public abstract class ByteScriptableStore<TState> : ScriptableObject, IStore<TState>
    {
        [SerializeField] private TState _initialState = default(TState);

        private readonly object _syncRoot = new object();
        private Dispatcher _dispatcher;
        private Reducer<TState> _reducer;
        private byte[] _lastState;
        private event Action _completedListener;
        private event Action<Exception> _errorListener;
        private event Action<TState> _nextListener;

        /// <summary>
        /// Dispatch an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public object Dispatch(object action)
        {
            return _dispatcher(action);
        }

        /// <summary>
        /// Get the state
        /// </summary>
        /// <returns></returns>
        public TState GetState() => BinarySerializer.Deserialize<TState>(_lastState);

        /// <summary>
        /// Subscribe to change state
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<TState> observer)
        {
            _completedListener -= observer.OnCompleted;
            _errorListener -= observer.OnError;
            _nextListener -= observer.OnNext;
            _completedListener += observer.OnCompleted;
            _errorListener += observer.OnError;
            _nextListener += observer.OnNext;

            var isError = false;
            try
            {
                if (_lastState != null) observer.OnNext(GetState());
            }
            catch (Exception e)
            {
                observer.OnError(e);
                isError = true;
            }
            if (!isError) observer.OnCompleted();

            return new Disposer(() =>
            {
                _completedListener -= observer.OnCompleted;
                _errorListener -= observer.OnError;
                _nextListener -= observer.OnNext;
            });
        }

        protected abstract Reducer<TState> InitReducer { get; }
        protected abstract Middleware<TState>[] InitMiddlewares { get; }

        private void OnEnable()
        {
            _reducer = InitReducer;
            _dispatcher = ApplyMiddlewares(InitMiddlewares);
            _lastState = BinarySerializer.Serialize(_initialState);
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
                _lastState = BinarySerializer.Serialize(_reducer(GetState(), action));
            }
            try
            {
                _nextListener?.Invoke(GetState());
                _completedListener?.Invoke();
            }
            catch (Exception e)
            {
                _errorListener?.Invoke(e);
            }
            return action;
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