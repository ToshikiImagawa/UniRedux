using UniRedux.Examples;
using UnityEngine;

namespace UniRedux.Provider.Examples
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