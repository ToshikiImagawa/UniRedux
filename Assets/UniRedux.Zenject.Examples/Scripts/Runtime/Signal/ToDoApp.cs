﻿using UniRedux.Provider;

namespace UniRedux.Zenject.Examples.Signal
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
        }
    }
}
