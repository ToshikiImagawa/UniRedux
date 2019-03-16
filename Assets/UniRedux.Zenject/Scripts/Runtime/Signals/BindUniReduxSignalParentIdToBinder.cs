using System;
using Zenject;

namespace UniRedux
{
    public class BindUniReduxSignalParentIdToBinder<TLocalState> : BindUniReduxSignalIdToBinder<TLocalState>
    {
        public BindUniReduxSignalParentIdToBinder(DiContainer container, UniReduxSignalBindingBindInfo signalBindInfo)
               : base(container, signalBindInfo)
        {
        }

        public BindUniReduxSignalIdToBinder<TLocalState> SetParent<TParentLocalState>(object identifier = null)
        {
            return SetParent(typeof(TParentLocalState), identifier);
        }
        public BindUniReduxSignalIdToBinder<TLocalState> SetParent(Type localStateType, object identifier = null)
        {
            SignalBindInfo.ParentBindingId = new UniReduxBindingId(localStateType, identifier);
            return this;
        }
    }
}
