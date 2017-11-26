namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonSortButton : Singleton.UI.SortButton
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
