using Zenject;

namespace UniRedux
{
    public class BindUniReduxSignalIdToBinder<TLocalState, TOriginalState> : BindUniReduxSignalToBinder<TLocalState, TOriginalState>
    {
        public BindUniReduxSignalIdToBinder(DiContainer container, UniReduxSignalBindingBindInfo signalBindInfo)
               : base(container, signalBindInfo)
        {
        }
        public BindUniReduxSignalToBinder<TLocalState, TOriginalState> WithId(object identifier)
        {
            SignalBindInfo.Identifier = identifier;
            return this;
        }
    }
}
