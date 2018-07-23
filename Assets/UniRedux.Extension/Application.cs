using UnityEngine;

namespace UniRedux
{
    /// <summary>
    /// SingletonApplication
    /// </summary>
    public abstract class Application<TState, TApplication> where TApplication : Application<TState, TApplication>, new()
    {
        private static TApplication _instance;
        private static readonly object SyncObj = new object();

        protected static TApplication Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncObj)
                {
                    if (_instance == null)
                    {
                        _instance = new TApplication();
                    }
                }
                return _instance;
            }
        }

        private readonly IStore<TState> _store;

        /// <summary>
        /// Current store
        /// </summary>
        /// <returns>Store</returns>
        public static IStore<TState> CurrentStore => Instance._store;

        /// <summary>
        /// Init Store
        /// </summary>
        protected abstract IStore<TState> InitStore { get; }

        protected Application()
        {
            if (InitStore == null) throw new System.ArgumentNullException(nameof(InitStore), "Cannot set property of null.");
            _store = InitStore;
        }
    }

    /// <summary>
    /// ScriptableObjectApplication
    /// </summary>
    public abstract class Application<TState> : ScriptableObject
    {
        private IStore<TState> _store;

        /// <summary>
        /// Current store
        /// </summary>
        /// <returns>Store</returns>
        public IStore<TState> CurrentStore => _store;

        /// <summary>
        /// Init Store
        /// </summary>
        protected abstract IStore<TState> InitStore { get; }

        private void OnEnable()
        {
            if (InitStore == null) throw new System.ArgumentNullException(nameof(InitStore), "Cannot set property of null.");
            _store = InitStore;
        }
    }
}