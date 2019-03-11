using System;
using Zenject;

namespace UniRedux
{
    [NoReflectionBaking]
    public class UniReduxSignalBindingBindInfo
    {
        public object Identifier { get; set; }
        public object ParentBindingId { get; set; }
        public Type LocalStateType { get; private set; }
        public Type OriginalStateType { get; private set; }

        public UniReduxSignalBindingBindInfo(Type localStateType, Type originalStateType)
        {
            LocalStateType = localStateType;
            OriginalStateType = originalStateType;
        }
    }
}
