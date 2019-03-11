using System;
using Zenject;

namespace UniRedux
{
    public class BindUniReduxSignalParentIdToBinder<TLocalState, TOriginalState> : BindUniReduxSignalIdToBinder<TLocalState, TOriginalState>
    {
        public BindUniReduxSignalParentIdToBinder(DiContainer container, UniReduxSignalBindingBindInfo signalBindInfo)
               : base(container, signalBindInfo)
        {
        }

        public BindUniReduxSignalIdToBinder<TLocalState, TOriginalState> SetParent<TParentLocalState, TParentOriginalState>(object identifier = null)
        {
            return SetParent(typeof(TParentLocalState), typeof(TParentOriginalState), identifier);
        }
        public BindUniReduxSignalIdToBinder<TLocalState, TOriginalState> SetParent(Type localStateType, Type originalStateType, object identifier = null)
        {
            SignalBindInfo.ParentBindingId = new UniReduxBindingId(localStateType, originalStateType, identifier);
            return this;
        }
    }
}
