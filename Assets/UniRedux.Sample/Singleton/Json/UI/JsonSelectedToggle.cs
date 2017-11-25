namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonSelectedToggle : Singleton.UI.SelectedToggle
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