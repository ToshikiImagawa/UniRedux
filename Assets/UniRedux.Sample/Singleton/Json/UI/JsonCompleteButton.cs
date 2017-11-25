namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonCompleteButton : Singleton.UI.CompleteButton
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
