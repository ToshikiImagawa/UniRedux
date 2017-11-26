namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinarySortButton : Singleton.UI.SortButton
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
