using System;
using UniSystem.Reactive.Disposables;

namespace UniRedux
{
    public class ImmutableEmptyObservable<T> : IObservable<T>
    {
        internal static readonly ImmutableEmptyObservable<T> Instance = new ImmutableEmptyObservable<T>();

        private ImmutableEmptyObservable()
        {
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}