namespace UniRedux.Sample.Singleton.Json.UI
{
    public class JsonOptionButton : Singleton.UI.OptionButton
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
