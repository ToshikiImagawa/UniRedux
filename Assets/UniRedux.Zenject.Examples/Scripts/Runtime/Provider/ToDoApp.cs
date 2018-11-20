using System;
using UniRedux.Provider;

namespace UniRedux.Zenject.Examples.Provider
{
    public static class ToDoApp
    {
        public static IUniReduxContainer ToDoViewContainer
            => UniReduxContainer<ToDoState>.Connect(state =>
                new ToDoLocalState
                {
                    ToDos = state.ToDos,
                    Filter = state.Filter
                });

        static ToDoApp()
        {
            UniReduxProvider.SetSetting(new ToDoProviderSetting());
        }
    }
}