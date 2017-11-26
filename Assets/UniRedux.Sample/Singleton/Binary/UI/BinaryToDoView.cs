namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinaryToDoView : Singleton.UI.ToDoView
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