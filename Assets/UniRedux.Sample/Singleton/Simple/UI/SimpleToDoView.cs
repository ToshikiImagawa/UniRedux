namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleToDoView : Singleton.UI.ToDoView
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