namespace UniRedux.Examples.EventSystem
{
    public static class ToDoApp
    {
        public static IStore<ToDoState> Store => UniReduxProvider.GetStore<ToDoState>();
        static ToDoApp()
        {
            UniReduxProvider.SetStore(Redux.CreateStore(
                ToDoReducer.Execute, ToDoReducer.InitState,
                UniReduxMiddleware.Logger,
                UniReduxMiddleware.CheckImmutableUpdate
            ));
        }
    }
}