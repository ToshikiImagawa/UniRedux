namespace UniRedux.Sample.Singleton.Simple.UI
{
    public class SimpleSortButton : Singleton.UI.SortButton
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
