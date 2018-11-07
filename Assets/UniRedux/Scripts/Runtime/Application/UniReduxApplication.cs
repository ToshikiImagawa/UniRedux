using UnityEngine;

namespace UniRedux
{
    public static class UniReduxApplication
    {
        private static readonly IReduxApplication Application;

        public static UniReduxApplication<TState> GetApplication<TState>()
        {
            var app = Application as UniReduxApplication<TState>;
            if (app == null)
                throw new ReduxException($"Not start boot ReduxApplication! -> state type:{nameof(TState)}");
            return app;
        }

        public static IReduxApplication GetApplication()
        {
            if (Application == null)
                throw new ReduxException($"Not start boot ReduxApplication!");
            return Application;
        }

        static UniReduxApplication()
        {
            Application = Resources.Load<ScriptableObject>("UniReduxApplication") as IReduxApplication;
            if (Application == null) throw new ReduxException("No definition file inheriting ReduxApplication!");
        }
    }

    public abstract class UniReduxApplication<TState> : ScriptableObject, IReduxApplication
    {
        /// <summary> Current store </summary>
        public virtual IStore<TState> CurrentStore { get; protected set; }

        /// <summary> Crate store </summary>
        protected abstract IStore<TState> CrateStore { get; }

        IStore IReduxApplication.CurrentStore => CurrentStore;

        private void OnEnable()
        {
            CurrentStore = CrateStore;
        }

        [ContextMenu("Reboot Application")]
        public void Reboot()
        {
            CurrentStore?.Dispose();
            CurrentStore = CrateStore;
        }

        [ContextMenu("Shutdown Application")]
        public void Shutdown()
        {
            if (CurrentStore == null) return;
            CurrentStore.Dispose();
            CurrentStore = null;
        }
    }

    public interface IReduxApplication
    {
        /// <summary> Current store </summary>
        IStore CurrentStore { get; }

        /// <summary> Reboot Application</summary>
        void Reboot();

        /// <summary> Shutdown　Application </summary>
        void Shutdown();
    }
}