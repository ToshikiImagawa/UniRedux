namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinaryCompleteButton : Singleton.UI.CompleteButton
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
