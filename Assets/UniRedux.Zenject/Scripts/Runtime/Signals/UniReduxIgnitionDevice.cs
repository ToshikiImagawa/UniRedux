using System;
using Zenject;

namespace UniRedux
{
    public class UniReduxIgnitionDevice : IInitializable, IDisposable
    {
        private readonly IStore _store;
        private readonly UniReduxSignalBus _signalBus;
        private IDisposable _disposable;

        public UniReduxIgnitionDevice(IStore store, UniReduxSignalBus signalBus)
        {
            _store = store;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _disposable = _store.Subscribe(ChangeState);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }

        private void ChangeState(VoidMessage voidMessage)
        {

        }
    }
}
