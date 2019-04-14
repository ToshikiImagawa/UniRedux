using UnityEngine.SceneManagement;

namespace UniRedux.Zenject.Examples
{
    public class ToDoApp
    {
        public IStore Store => UniReduxProvider.Store;
        public ToDoApp()
        {
            UniReduxProvider.SetStore(Redux.CreateStore(
                ToDoReducer.Execute, ToDoReducer.InitState,
                UniReduxMiddleware.Logger,
                UniReduxMiddleware.CheckImmutableUpdate
            ));

            SceneManager.LoadScene("SignalToDoMiller", LoadSceneMode.Additive);
        }
    }
}
