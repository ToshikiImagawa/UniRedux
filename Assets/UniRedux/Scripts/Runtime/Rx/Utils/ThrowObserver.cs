using System;

namespace UniRedux
{
    public class ThrowObserver<T> : IObserver<T>
    {
        public static readonly ThrowObserver<T> Instance = new ThrowObserver<T>();

        private ThrowObserver()
        {
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            error.Throw();
        }

        public void OnNext(T value)
        {
        }
    }
}