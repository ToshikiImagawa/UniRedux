namespace UniRedux.Examples.Application
{
    public class OptionButton : OptionButtonBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}