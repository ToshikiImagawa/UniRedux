using System;

namespace UniRedux
{
    /// <summary>
    /// Component
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public delegate Func<TState, TResult> Component<in TState, in TSource, out TResult>(TSource source);
}