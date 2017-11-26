namespace UniRedux.Sample.Singleton.Binary.UI
{
    public class BinarySelectedToggle : Singleton.UI.SelectedToggle
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