namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleSelectedToggle : Singleton.UI.SelectedToggle
    {
        protected override IStore<ToDoState> CurrentStore
        {
            get
            {
                return ToDoApplication.CurrentStore;
            }
        }
    }
}