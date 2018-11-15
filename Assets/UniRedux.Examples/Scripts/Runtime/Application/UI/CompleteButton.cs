namespace UniRedux.Examples.Application
{
    public class CompleteButton : CompleteButtonBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}