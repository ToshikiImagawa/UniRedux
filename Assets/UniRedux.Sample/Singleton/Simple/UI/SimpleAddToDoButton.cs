namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleAddToDoButton : Singleton.UI.AddToDoButton
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
