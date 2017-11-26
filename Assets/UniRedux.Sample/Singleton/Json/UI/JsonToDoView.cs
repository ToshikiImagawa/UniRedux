namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonToDoView : Singleton.UI.ToDoView
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