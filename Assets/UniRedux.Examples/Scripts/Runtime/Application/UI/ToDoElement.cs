namespace UniRedux.Examples.Application
{
    public class ToDoElement : ToDoElementBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}