namespace UniRedux.Sample.Singleton.Simple
{
    public class ToDoApplication : Application<ToDoState, ToDoApplication>
    {
        protected override IStore<ToDoState> InitStore
        {
            get
            {
                return Redux.CreateStore(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger, UniReduxMiddleware.CheckImmutableUpdate);
            }
        }
    }
}
