using System;

namespace UniSystem.Reactive.Disposables
{
    public static class Disposable
    {
        public static IDisposable Empty => EmptyDisposable.Instance;

        public static IDisposable Create(Action dispose)
        {
            return new ActionDisposable(dispose);
        }

        private class EmptyDisposable : IDisposable
        {
            private static EmptyDisposable _instance;
            public static EmptyDisposable Instance => _instance ?? (_instance = new EmptyDisposable());

            public void Dispose()
            {
            }
        }

        private class ActionDisposable : IDisposable
        {
            private bool _disposedValue;
            private readonly Action _dispose;

            public ActionDisposable(Action dispose)
            {
                _dispose = dispose;
            }

            private void Dispose(bool disposing)
            {
                if (_disposedValue) return;
                if (disposing)
                {
                    _dispose?.Invoke();
                }

                _disposedValue = true;
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
            }
        }
    }
}