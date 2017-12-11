using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace UniRedux
{
    /// <summary>
    /// Redux
    /// </summary>
    public static class Redux
    {
        public static IStore<TState> CreateStore<TState>(Reducer<TState> reducer, TState preloadedState = default(TState), params Middleware<TState>[] enhancer)
        {
            return new Store<TState>(reducer, preloadedState, enhancer);
        }

        public static IStore<TState> CreateDeepFreezeStore<TState>(Reducer<TState> reducer, TState preloadedState = default(TState), params Middleware<TState>[] enhancer)
        {
            return new DeepFreezeStoreByJson<TState>(reducer, preloadedState, enhancer);
        }
        public static IStore<TState> CreateDeepFreezeStoreBinary<TState>(Reducer<TState> reducer, TState preloadedState = default(TState), params Middleware<TState>[] enhancer)
        {
            return new DeepFreezeStoreBinary<TState>(reducer, preloadedState, enhancer);
        }

        #region Array
        public static T[] InsertItem<T>(this T[] array, int index, T value)
        {
            if (array.Length < index || index < 0) throw new ArgumentOutOfRangeException();
            var isLast = array.Length == index;
            if (isLast) return array.AddItem(value);
            var newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, index);
            newArray[index] = value;
            Array.Copy(array, index + 1, newArray, index + 1, array.Length - index);
            return newArray;
        }
        public static T[] AddItem<T>(this T[] array, T value)
        {
            var newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = value;
            return newArray;
        }
        public static T[] RemoveItem<T>(this T[] array, int index)
        {
            if (array.Length <= index || index < 0) throw new ArgumentOutOfRangeException();
            if (array.Length == 1) return new T[0];
            var newArray = new T[array.Length - 1];
            Array.Copy(array, newArray, index);
            if (array.Length - 1 == index) return newArray;
            Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);
            return newArray;
        }
        public static T[] RemoveItem<T>(this T[] array, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException();
            var list = array.ToList();
            list.RemoveAll(match);
            return list.ToArray();
        }
        public static T[] UpdateItem<T>(this T[] array, int index, T item)
        {
            var copyArray = array.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = item;
            }
            return copyArray;
        }
        public static T[] UpdateItem<T>(this T[] array, int index, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException();
            var copyArray = array.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = updater(copyArray[index]);
            }
            return copyArray;
        }
        public static T[] UpdateItem<T>(this T[] array, Func<T, T> updater, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException();
            if (updater == null) throw new ArgumentNullException();
            return array.Select(target => match(target) ? updater(target) : target).ToArray();
        }
        public static T[] UpdateItem<T>(this T[] array, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException();
            return array.Select(target => updater(target)).ToArray();
        }
        #endregion
        #region Enumerable
        public static IEnumerable<T> InsertItem<T>(this IEnumerable<T> items, int index, T value)
        {
            var list = items.ToList();
            list.Insert(index, value);
            return list.ToArray();
        }
        public static IEnumerable<T> AddItem<T>(this IEnumerable<T> eumerable, T item)
        {
            var list = eumerable.ToList();
            list.Add(item);
            return list.ToArray();
        }
        public static IEnumerable<T> RemoveItem<T>(this IEnumerable<T> eumerable, int index)
        {
            var list = eumerable.ToList();
            list.RemoveAt(index);
            return list.ToArray();
        }
        public static IEnumerable<T> RemoveItem<T>(this IEnumerable<T> eumerable, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            var list = eumerable.ToList();
            list.RemoveAll(match);
            return list.ToArray();
        }
        public static IEnumerable<T> UpdateItem<T>(this IEnumerable<T> eumerable, int index, T item)
        {
            var copyArray = eumerable.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = item;
            }
            return copyArray;
        }
        public static IEnumerable<T> UpdateItem<T>(this IEnumerable<T> eumerable, int index, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            var copyArray = eumerable.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = updater(copyArray[index]);
            }
            return copyArray;
        }
        public static IEnumerable<T> UpdateItem<T>(this IEnumerable<T> eumerable, Func<T, T> updater, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return eumerable.Select(target => match(target) ? updater(target) : target).ToArray();
        }
        public static IEnumerable<T> UpdateItem<T>(this IEnumerable<T> eumerable, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return eumerable.Select(target => updater(target)).ToArray();
        }
        #endregion
        #region Dictionary
        public static IDictionary<TKey, TValue> AddItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (dictionary.ContainsKey(key)) throw new ArgumentException(nameof(dictionary));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary.Add(key, item);
            return newDictionary;
        }
        public static IDictionary<TKey, TValue> RemoveItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary.Remove(key);
            return newDictionary;
        }
        public static IDictionary<TKey, TValue> RemoveItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Predicate<TValue> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            return dictionary.Where(item => match(item.Value)).ToDictionary(item => item.Key, item => item.Value);
        }
        public static IDictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary[key] = item;
            return newDictionary;
        }
        public static IDictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updater)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (dictionary.ContainsKey(key)) throw new ArgumentException(nameof(dictionary));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary[key] = updater(newDictionary[key]);
            return newDictionary;
        }
        public static IDictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TValue> updater, Predicate<TValue> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return dictionary.Select(target => match(target.Value) ? new KeyValuePair<TKey, TValue>(target.Key, updater(target.Value)) : target).ToDictionary(item => item.Key, item => item.Value);
        }
        public static IDictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, Func<TValue, TValue> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return dictionary.Select(target => new KeyValuePair<TKey, TValue>(target.Key, updater(target.Value))).ToDictionary(item => item.Key, item => item.Value);
        }
        #endregion

    }

    /// <summary>
    /// Store
    /// </summary>
    public interface IStore<TState> : IObservable<TState>
    {
        /// <summary>
        /// Dispatch an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        object Dispatch(object action);

        /// <summary>
        /// Get the state
        /// </summary>
        /// <returns></returns>
        TState GetState();

        /// <summary>
        /// Replace reducer
        /// </summary>
        /// <param name="nextReducer"></param>
        void ReplaceReducer(Reducer<TState> nextReducer);
    }

    internal class Store<TState> : IStore<TState>
    {
        private readonly object _syncRoot = new object();
        private readonly Dispatcher _dispatcher;
        private Reducer<TState> _reducer;
        private TState _lastState;
        private event Action _completedListener;
        private event Action<Exception> _errorListener;
        private event Action<TState> _nextListener;
        private bool isError = false;

        object IStore<TState>.Dispatch(object action)
        {
            return _dispatcher(action);
        }

        TState IStore<TState>.GetState() => _lastState;

        void IStore<TState>.ReplaceReducer(Reducer<TState> nextReducer)
        {
            _reducer = nextReducer;
        }

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

            if (_lastState != null) observer.OnNext(_lastState);

            return new Disposer(() =>
            {
                _completedListener -= observer.OnCompleted;
                _errorListener -= observer.OnError;
                _nextListener -= observer.OnNext;
            });
        }

        public Store(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] enhancer)
        {
            _reducer = reducer;
            _dispatcher = ApplyMiddlewares(enhancer);

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

    internal class DeepFreezeStoreByJson<TState> : SerializeStore<TState, string>
    {
        public DeepFreezeStoreByJson(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] enhancer) : base(reducer, initialState, enhancer)
        {
        }

        protected override TState Deserialize(string serializeState)
        {
            return UnityEngine.JsonUtility.FromJson<TState>(serializeState);
        }

        protected override string Serialize(TState state)
        {
            return UnityEngine.JsonUtility.ToJson(state);
        }
    }

    internal class DeepFreezeStoreBinary<TState> : SerializeStore<TState, byte[]>
    {
        private BinaryFormatter binaryFormatter = new BinaryFormatter();

        public DeepFreezeStoreBinary(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] enhancer) : base(reducer, initialState, enhancer)
        {
#if UNITY_IPHONE
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
        }

        protected override TState Deserialize(byte[] serializeState)
        {
            using (var mem = new MemoryStream(serializeState))
            {
                return (TState)binaryFormatter.Deserialize(mem);
            }
        }

        protected override byte[] Serialize(TState state)
        {
            using (var mem = new MemoryStream())
            {
                binaryFormatter.Serialize(mem, state);
                mem.Position = 0;
                return mem.ToArray();
            }
        }
    }

    internal abstract class SerializeStore<TState, TSerializeState> : IStore<TState>
    {
        private readonly object _syncRoot = new object();
        private readonly Dispatcher _dispatcher;
        private Reducer<TState> _reducer;
        private TSerializeState _lastState;
        private event Action _completedListener;
        private event Action<Exception> _errorListener;
        private event Action<TState> _nextListener;

        /// <summary>
        /// Dispatch an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        object IStore<TState>.Dispatch(object action)
        {
            return _dispatcher(action);
        }

        /// <summary>
        /// Get the state
        /// </summary>
        /// <returns></returns>
        TState IStore<TState>.GetState() => Deserialize(_lastState);

        void IStore<TState>.ReplaceReducer(Reducer<TState> nextReducer)
        {
            _reducer = nextReducer;
        }

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

            try
            {
                if (_lastState != null) observer.OnNext(Deserialize(_lastState));
            }
            catch (Exception e)
            {
                observer.OnError(e);
            }

            return new Disposer(() =>
            {
                _completedListener -= observer.OnCompleted;
                _errorListener -= observer.OnError;
                _nextListener -= observer.OnNext;
            });
        }

        public SerializeStore(Reducer<TState> reducer, TState initialState = default(TState), params Middleware<TState>[] enhancer)
        {
            _reducer = reducer;
            _dispatcher = ApplyMiddlewares(enhancer);

            _lastState = Serialize(initialState);
        }

        protected abstract TSerializeState Serialize(TState state);
        protected abstract TState Deserialize(TSerializeState serializeState);

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
                _lastState = Serialize(_reducer(Deserialize(_lastState), action));
            }
            try
            {
                _nextListener?.Invoke(Deserialize(_lastState));
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