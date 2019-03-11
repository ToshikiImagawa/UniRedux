using System;

namespace UniRedux
{
    public class UniReduxSignalParentIdCopyBinder : UniReduxSignalIdCopyBinder
    {
        public UniReduxSignalParentIdCopyBinder(
               UniReduxSignalDeclarationBindInfo signalBindInfo)
               : base(signalBindInfo)
        {
        }

        public UniReduxSignalIdCopyBinder SetParent<TLocalState, TOriginalState>(object identifier = null)
        {
            return SetParent(typeof(TLocalState), typeof(TOriginalState), identifier);
        }

        public UniReduxSignalIdCopyBinder SetParent(Type localStateType, Type originalStateType, object identifier = null)
        {
            SignalBindInfo.ParentBindingId = new UniReduxBindingId(localStateType, originalStateType, identifier);
            return this;
        }
    }
}
