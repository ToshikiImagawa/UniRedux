using System;

namespace UniRedux
{
    public static class SubjectExtensions
    {
        private class AnonymousSubject<T, U> : ISubject<T, U>
        {
            private readonly IObserver<T> _observer;
            private readonly IObservable<U> _observable;

            public AnonymousSubject(IObserver<T> observer, IObservable<U> observable)
            {
                _observer = observer;
                _observable = observable;
            }

            public void OnCompleted()
            {
                _observer.OnCompleted();
            }

            public void OnError(Exception error)
            {
                if (error == null) throw new ArgumentNullException(nameof(error));

                _observer.OnError(error);
            }

            public void OnNext(T value)
            {
                _observer.OnNext(value);
            }

            public IDisposable Subscribe(IObserver<U> observer)
            {
                if (observer == null) throw new ArgumentNullException(nameof(observer));

                return _observable.Subscribe(observer);
            }
        }

        private class AnonymousSubject<T> : AnonymousSubject<T, T>, ISubject<T>
        {
            public AnonymousSubject(IObserver<T> observer, IObservable<T> observable)
                : base(observer, observable)
            {
            }
        }
    }
}