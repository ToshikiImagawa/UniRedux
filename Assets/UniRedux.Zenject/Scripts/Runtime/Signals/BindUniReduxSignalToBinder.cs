using System;
using Zenject;

namespace UniRedux
{
    public class BindUniReduxSignalToBinder<TLocalState>
    {
        private readonly DiContainer _container;
        private readonly BindStatement _bindStatement;
        private readonly UniReduxSignalBindingBindInfo _signalBindInfo;

        protected UniReduxSignalBindingBindInfo SignalBindInfo => _signalBindInfo;

        public BindUniReduxSignalToBinder(DiContainer container, UniReduxSignalBindingBindInfo signalBindInfo)
        {
            _container = container;
            _signalBindInfo = signalBindInfo;
            _bindStatement = container.StartBinding();
        }
        public UniReduxSignalCopyBinder ToMethod(Action<TLocalState> callback)
        {
            if (_bindStatement.HasFinalizer) throw Assert.CreateException();
            _bindStatement.SetFinalizer(new NullBindingFinalizer());

            var bindInfo = _container.Bind<IDisposable>()
                .To<SignalCallbackWrapper>()
                .AsCached()
                .WithArguments(_signalBindInfo, (Action<object>)(o => callback((TLocalState)o)))
                .NonLazy().BindInfo;

            return new UniReduxSignalCopyBinder(bindInfo);
        }

        public UniReduxSignalCopyBinder ToMethod(Action callback)
        {
            return ToMethod(signal => callback());
        }
    }
}
