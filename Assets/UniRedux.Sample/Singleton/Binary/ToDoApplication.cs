namespace UniRedux.Sample.Singleton.Binary
{
    public class ToDoApplication : Application<ToDoState, ToDoApplication>
    {
        protected override IStore<ToDoState> InitStore
        {
            get
            {
                return Redux.CreateDeepFreezeStoreBinary(ToDoReducer.Execute, ToDoReducer.InitState, UniReduxMiddleware.Logger, UniReduxMiddleware.CheckImmutableUpdate);
            }
        }
    }
}