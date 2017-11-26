namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinaryOptionButton : Singleton.UI.OptionButton
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
