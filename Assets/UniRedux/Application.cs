namespace UniRedux
{
    public interface IApplication<TState>
    {
        IStore<TState> CurrentStore { get; }
    }
}