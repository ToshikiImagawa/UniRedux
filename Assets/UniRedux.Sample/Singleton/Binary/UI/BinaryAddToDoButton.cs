namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinaryAddToDoButton : Singleton.UI.AddToDoButton
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
