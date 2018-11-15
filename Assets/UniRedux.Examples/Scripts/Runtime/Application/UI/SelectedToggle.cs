namespace UniRedux.Examples.Application
{
    public class SelectedToggle : SelectedToggleBase
    {
        protected override IStore<ToDoState> CurrentStore =>
            UniReduxApplication.GetApplication<ToDoState>().CurrentStore;
    }
}