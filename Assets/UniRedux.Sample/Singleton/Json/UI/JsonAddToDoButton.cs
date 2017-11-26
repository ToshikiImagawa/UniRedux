namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonAddToDoButton : Singleton.UI.AddToDoButton
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
