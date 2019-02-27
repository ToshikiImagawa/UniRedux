using System;

namespace UniRedux.Provider
{
    public class UniReduxSignalDeclarationBindInfo
    {
        public Type SignalType { get; }

        public UniReduxSignalDeclarationBindInfo(Type signalType)
        {
            SignalType = signalType;
        }
    }
}