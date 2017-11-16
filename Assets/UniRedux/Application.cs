using UnityEngine;
namespace UniRedux
{
    /// <summary>
    /// Application
    /// </summary>
    public abstract class Application<TState, TApplication> : MonoBehaviour where TApplication : Application<TState, TApplication>
    {
        private static TApplication instance;
        protected static TApplication Instance
        {
            get
            {
                if (instance == null)
                {
                    var t = typeof(TApplication);

                    instance = (TApplication)FindObjectOfType(t);
                    if (instance == null)
                    {
                        Debug.LogError($"There is no GameObject attaching {t}.");
                    }
                }

                return instance;
            }
        }

        private IStore<TState> _store;

        /// <summary>
        /// Current store
        /// </summary>
        /// <returns>Store</returns>
        public static IStore<TState> CurrentStore => Instance._store;

        protected abstract IStore<TState> InitStore { get; }

        virtual protected void Awake()
        {
            if (this != Instance)
            {
                Destroy(this);
                Debug.LogError(
                    $"{typeof(TApplication)} has already been attached to another GameObject, we have discarded the component." +
                    $"The attached GameObject is {Instance.gameObject.name}."
                );
                return;
            }
            _store = InitStore;
        }
    }
}