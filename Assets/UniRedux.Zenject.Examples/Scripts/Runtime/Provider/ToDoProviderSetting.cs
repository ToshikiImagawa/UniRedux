using UniRedux.Provider;

namespace UniRedux.Zenject.Examples.Provider
{
    public class ToDoProviderSetting : UniReduxProvider.Setting
    {
        public override void Initialize()
        {
            SetStore(Redux.CreateStore(
                ToDoReducer.Execute, ToDoReducer.InitState,
                UniReduxMiddleware.Logger,
                UniReduxMiddleware.CheckImmutableUpdate
            ));
        }
    }
}