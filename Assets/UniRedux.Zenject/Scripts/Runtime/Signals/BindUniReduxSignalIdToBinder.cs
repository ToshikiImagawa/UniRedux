using Zenject;

namespace UniRedux
{
    public class BindUniReduxSignalIdToBinder<TLocalState> : BindUniReduxSignalToBinder<TLocalState>
    {
        public BindUniReduxSignalIdToBinder(DiContainer container, UniReduxSignalBindingBindInfo signalBindInfo)
               : base(container, signalBindInfo)
        {
        }
        public BindUniReduxSignalToBinder<TLocalState> WithId(object identifier)
        {
            SignalBindInfo.Identifier = identifier;
            return this;
        }
    }
}
