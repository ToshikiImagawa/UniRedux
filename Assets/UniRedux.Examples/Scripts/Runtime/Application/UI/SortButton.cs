namespace UniRedux.Examples.Application
{
    public class SortButton : SortButtonBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}