using System;
using Zenject;

namespace UniRedux
{
    [NoReflectionBaking]
    public class UniReduxSignalDeclarationBindInfo
    {
        public object Identifier
        {
            get; set;
        }

        public UniReduxBindingId ParentBindingId
        {
            get; set;
        }

        public Type LocalStateType
        {
            get; private set;
        }

        public IUniReduxStateDirector Director
        {
            get; private set;
        }

        public UniReduxSignalDeclarationBindInfo(Type localStateType, IUniReduxStateDirector director)
        {
            LocalStateType = localStateType;
            Director = director;
        }
    }
}
