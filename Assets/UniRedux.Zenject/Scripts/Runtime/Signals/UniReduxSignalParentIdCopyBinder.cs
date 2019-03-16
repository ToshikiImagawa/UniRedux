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

        public UniReduxSignalIdCopyBinder SetParent<TLocalState>(object identifier = null)
        {
            return SetParent(typeof(TLocalState), identifier);
        }

        public UniReduxSignalIdCopyBinder SetParent(Type localStateType, object identifier = null)
        {
            SignalBindInfo.ParentBindingId = new UniReduxBindingId(localStateType, identifier);
            return this;
        }
    }
}
