using System;

namespace UniRedux.Rx
{
    public static class Observable
    {
        public static IObservable<T> Empty<T>()
        {
            return ImmutableEmptyObservable<T>.Instance;
        }
    }
}