using System;

namespace UniRedux.Provider
{
    public static class UniReduxProvider
    {
        private static IStore _store;
        private static Setting _setting;

        /// <summary>
        /// State type
        /// </summary>
        public static RuntimeTypeHandle StateType { get; private set; }

        /// <summary>
        /// Store
        /// </summary>
        /// <exception cref="ReduxException"></exception>
        public static IStore Store
        {
            get
            {
                if (_store == null)
                    throw Assert.CreateException(
                        "Store is not set. Please call SetStore() in the setting file constructor.");
                return _store;
            }
        }

        public static void SetSetting(Setting setting, bool autoBooting = true)
        {
            if (_setting != null) return;
            _setting = setting;
            if (_setting == null) throw Assert.CreateException("UniReduxProvider setting file is not defined.");
            if (autoBooting) Boot();
        }

        /// <summary>
        /// Boot
        /// </summary>
        /// <exception cref="ReduxException"></exception>
        public static void Boot()
        {
            _setting?.Initialize();
            if (_store == null)
                throw Assert.CreateException(
                    "Store is not set. Please call SetStore() in the setting file constructor.");
        }

        /// <summary>
        /// Shutdown
        /// </summary>
        public static void Shutdown()
        {
            _store?.Dispose();
            _store = null;
        }

        /// <summary>
        /// Store
        /// </summary>
        /// <exception cref="ReduxException"></exception>
        internal static IStore<TState> GetStore<TState>()
        {
            if (!typeof(TState).TypeHandle.Equals(StateType))
                throw Assert.CreateException($"The type of State is different. {typeof(TState)} != {StateType}(Store)");

            return Store as IStore<TState>;
        }

        /// <summary>
        /// Provider Setting
        /// </summary>
        public abstract class Setting
        {
            protected static void SetStore<TState>(IStore<TState> store)
            {
                _store?.Dispose();
                _store = store;
                StateType = typeof(TState).TypeHandle;
            }

            public abstract void Initialize();
        }
    }
}