namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinaryToDoElement : Singleton.UI.ToDoElement
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
