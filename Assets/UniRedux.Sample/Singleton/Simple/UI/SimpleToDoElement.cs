namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleToDoElement : Singleton.UI.ToDoElement
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
