namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleOptionButton : Singleton.UI.OptionButton
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
