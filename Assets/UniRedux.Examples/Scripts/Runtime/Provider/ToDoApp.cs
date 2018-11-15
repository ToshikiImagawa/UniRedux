using UniRedux.Examples;

namespace UniRedux.Provider.Examples
{
    public static class ToDoApp
    {
        public static IUniReduxContainer ToDoViewStateStateContainer { get; }

        static ToDoApp()
        {
            UniReduxProvider.SetSetting(new ToDoProviderSetting());
            ToDoViewStateStateContainer = UniReduxContainer<ToDoState>.Connect(
            state => new ToDoLocalState
            {
                ToDos = state.ToDos,
                Filter = state.Filter
            }, dispatcher => new ToDoActionDispatcher(dispatcher));
        }
    }
}