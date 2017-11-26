namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonToDoElement : Singleton.UI.ToDoElement
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
