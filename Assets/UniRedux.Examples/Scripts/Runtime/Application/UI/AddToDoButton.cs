namespace UniRedux.Examples.Application
{
    public class AddToDoButton : AddToDoButtonBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}