namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleCompleteButton : Singleton.UI.CompleteButton
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
