using System;

namespace UniRedux
{
    /// <summary>
    /// Dispatcher
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public delegate object Dispatcher(object action);

    /// <summary>
    /// Middleware
    /// </summary>
    /// <param name="store"></param>
    /// <returns></returns>
    public delegate Func<Dispatcher, Dispatcher> Middleware<TState>(IStore<TState> store);

    /// <summary>
    /// Reducer
    /// </summary>
    /// <param name="previousState"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public delegate TState Reducer<TState>(TState previousState, object action);
}