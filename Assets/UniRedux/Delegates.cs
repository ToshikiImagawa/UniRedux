using System;

namespace UniRedux
{
    public delegate object Dispatcher(object action);

    public delegate Func<Dispatcher, Dispatcher> Middleware<TState>(IStore<TState> store);

    public delegate TState Reducer<TState>(TState previousState, object action);
}