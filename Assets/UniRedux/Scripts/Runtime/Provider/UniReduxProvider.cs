using System;

namespace UniRedux.Provider
{
    public static class UniReduxProvider
    {
        private static IStore _store;

        /// <summary>
        /// State type
        /// </summary>
        private static RuntimeTypeHandle StateType { get; set; }

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

        /// <summary>
        /// Set store
        /// </summary>
        /// <param name="store"></param>
        /// <typeparam name="TState"></typeparam>
        public static void SetStore<TState>(IStore<TState> store)
        {
            _store?.Dispose();
            _store = store;
            StateType = typeof(TState).TypeHandle;
        }

        /// <summary>
        /// Get store
        /// </summary>
        /// <exception cref="ReduxException"></exception>
        public static IStore<TState> GetStore<TState>()
        {
            if (!typeof(TState).TypeHandle.Equals(StateType))
                throw Assert.CreateException($"The type of State is different. {typeof(TState)} != {StateType}(Store)");

            return Store as IStore<TState>;
        }
    }
}