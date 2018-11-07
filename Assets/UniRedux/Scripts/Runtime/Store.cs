using System;
using System.Collections.Generic;
using System.Linq;
using UniSystem.Reactive.Disposables;

namespace UniRedux
{
    /// <summary>
    /// Redux
    /// </summary>
    public static class Redux
    {
        public static IStore<TState> CreateStore<TState>(Reducer<TState> reducer,
            TState preloadState = default(TState), params Middleware<TState>[] enhancer)
        {
            return new Store<TState>(reducer, preloadState, enhancer);
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
            return array.Select(updater).ToArray();
        }

        #endregion

        #region Enumerable

        public static T[] InsertItem<T>(this IEnumerable<T> items, int index, T value)
        {
            var list = items.ToList();
            list.Insert(index, value);
            return list.ToArray();
        }

        public static T[] AddItem<T>(this IEnumerable<T> self, T item)
        {
            var list = self.ToList();
            list.Add(item);
            return list.ToArray();
        }

        public static T[] RemoveItem<T>(this IEnumerable<T> self, int index)
        {
            var list = self.ToList();
            list.RemoveAt(index);
            return list.ToArray();
        }

        public static T[] RemoveItem<T>(this IEnumerable<T> self, Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            var list = self.ToList();
            list.RemoveAll(match);
            return list.ToArray();
        }

        public static T[] UpdateItem<T>(this IEnumerable<T> self, int index, T item)
        {
            var copyArray = self.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = item;
            }

            return copyArray;
        }

        public static T[] UpdateItem<T>(this IEnumerable<T> self, int index, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            var copyArray = self.ToArray();
            if (copyArray.Length > index && index >= 0)
            {
                copyArray[index] = updater(copyArray[index]);
            }

            return copyArray;
        }

        public static T[] UpdateItem<T>(this IEnumerable<T> self, Func<T, T> updater,
            Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return self.Select(target => match(target) ? updater(target) : target).ToArray();
        }

        public static T[] UpdateItem<T>(this IEnumerable<T> self, Func<T, T> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return self.Select(updater).ToArray();
        }

        #endregion

        #region Dictionary

        public static Dictionary<TKey, TValue> AddItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key, TValue item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (dictionary.ContainsKey(key)) throw new ArgumentException(nameof(dictionary));
            return new Dictionary<TKey, TValue>(dictionary) {{key, item}};
        }

        public static Dictionary<TKey, TValue> RemoveItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary.Remove(key);
            return newDictionary;
        }

        public static Dictionary<TKey, TValue> RemoveItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Predicate<TValue> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            return dictionary.Where(item => !match(item.Value)).ToDictionary(item => item.Key, item => item.Value);
        }

        public static Dictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key, TValue item)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary) {[key] = item};
            return newDictionary;
        }

        public static Dictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            TKey key, Func<TValue, TValue> updater)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (dictionary.ContainsKey(key)) throw new ArgumentException(nameof(dictionary));
            var newDictionary = new Dictionary<TKey, TValue>(dictionary);
            newDictionary[key] = updater(newDictionary[key]);
            return newDictionary;
        }

        public static Dictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Func<TValue, TValue> updater, Predicate<TValue> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return dictionary
                .Select(target =>
                    match(target.Value) ? new KeyValuePair<TKey, TValue>(target.Key, updater(target.Value)) : target)
                .ToDictionary(item => item.Key, item => item.Value);
        }

        public static Dictionary<TKey, TValue> UpdateItem<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Func<TValue, TValue> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            return dictionary.Select(target => new KeyValuePair<TKey, TValue>(target.Key, updater(target.Value)))
                .ToDictionary(item => item.Key, item => item.Value);
        }

        #endregion
    }

    /// <summary>
    /// Store
    /// </summary>
    public interface IStore<TState> : IStore
    {
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

    public interface IStore : IObservable<VoidMessage>, IDisposable
    {
        /// <summary>
        /// Get the state
        /// </summary>
        /// <returns></returns>
        object GetState(Type type);

#if UNITY_EDITOR
        /// <summary>
        /// Get the state force
        /// </summary>
        /// <returns></returns>
        object GetStateForce();
#endif

        /// <summary>
        /// Dispatch an action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        object Dispatch(object action);
    }

    internal class Store<TState> : IStore<TState>
    {
        private readonly object _disposeLock = new object();
        private ListObserver<VoidMessage> _listObserver = ListObserver.Empty<VoidMessage>();

        private readonly object _syncRoot = new object();
        private readonly Dispatcher _dispatcher;
        private Reducer<TState> _reducer;
        private TState _lastState;
        private bool _isStopped = false;
        private Exception _lastError;

        public object GetState(Type type)
        {
            if (type != typeof(TState)) throw Assert.CreateException("It is different from the type of State");
            return _lastState;
        }

#if UNITY_EDITOR
        object IStore.GetStateForce()
        {
            return _lastState;
        }
#endif

        public object Dispatch(object action)
        {
            return _isStopped ? action : _dispatcher.Invoke(action);
        }

        public TState GetState() => _lastState;

        public void ReplaceReducer(Reducer<TState> nextReducer)
        {
            lock (_syncRoot)
            {
                _reducer = nextReducer;
            }
        }

        public IDisposable Subscribe(IObserver<VoidMessage> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            var error = default(Exception);
            var subscription = default(Subscription);
            lock (_syncRoot)
            {
                if (_isStopped)
                {
                    error = _lastError;
                }
                else
                {
                    if (_listObserver == null) _listObserver = ListObserver.Empty<VoidMessage>();
                    _listObserver = _listObserver.Add(observer);
                    subscription = new Subscription(this, observer);
                }
            }

            if (subscription != null)
            {
                observer.OnNext(VoidMessage.Default);
                return subscription;
            }

            if (error != null)
            {
                observer.OnError(error);
            }

            return Disposable.Empty;
        }

        public void Dispose()
        {
            lock (_disposeLock)
            {
                if (_isStopped) return;
                ListObserver<VoidMessage> errorListObserver;
                lock (_syncRoot)
                {
                    _isStopped = true;
                    if (_listObserver == null) return;
                    errorListObserver = _listObserver;
                    _listObserver = null;
                }

                if (_lastError != null)
                {
                    errorListObserver?.OnError(_lastError);
                }
                else
                {
                    errorListObserver?.OnCompleted();
                }
            }
        }

        public Store(Reducer<TState> reducer, TState initialState = default(TState),
            params Middleware<TState>[] enhancer)
        {
            _reducer = reducer;
            _dispatcher = ApplyMiddlewares(enhancer);

            _lastState = initialState;
        }

        private Dispatcher ApplyMiddlewares(params Middleware<TState>[] middlewares)
        {
            return middlewares.Aggregate<Middleware<TState>, Dispatcher>(InnerDispatch,
                (current, middleware) => middleware(this)(current));
        }

        private object InnerDispatch(object action)
        {
            if (_isStopped) return action;
            var errorListObserver = default(ListObserver<VoidMessage>);
            lock (_syncRoot)
            {
                try
                {
                    _lastState = _reducer(_lastState, action);
                }
                catch (Exception e)
                {
                    _lastError = e;
                    _isStopped = true;
                    errorListObserver = _listObserver;
                    _listObserver = null;
                }
            }

            if (!_isStopped)
            {
                _listObserver?.OnNext(new VoidMessage());
            }
            else if (_lastError != null)
            {
                errorListObserver?.OnError(_lastError);
            }

            return action;
        }

        private sealed class Subscription : IDisposable
        {
            private readonly object _disposeLock = new object();
            private Store<TState> _parent;
            private IObserver<VoidMessage> _unsubscribeTarget;

            public Subscription(Store<TState> parent, IObserver<VoidMessage> unsubscribeTarget)
            {
                _parent = parent;
                _unsubscribeTarget = unsubscribeTarget;
            }

            public void Dispose()
            {
                lock (_disposeLock)
                {
                    if (_parent == null) return;
                    lock (_parent._syncRoot)
                    {
                        if (_unsubscribeTarget != null)
                        {
                            if (_parent._listObserver == null) return;
                            _parent._listObserver = _parent._listObserver.Remove(_unsubscribeTarget);
                        }

                        _unsubscribeTarget = null;
                        _parent = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Void data for message
    /// </summary>
    public struct VoidMessage : IEquatable<VoidMessage>
    {
        public static VoidMessage Default { get; } = new VoidMessage();

        public static bool operator ==(VoidMessage first, VoidMessage second)
        {
            return true;
        }

        public static bool operator !=(VoidMessage first, VoidMessage second)
        {
            return false;
        }

        public bool Equals(VoidMessage other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is VoidMessage;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}