namespace UniRedux
{
    public class UniReduxSignalIdCopyBinder : UniReduxSignalCopyBinder
    {
        public UniReduxSignalIdCopyBinder(
            UniReduxSignalDeclarationBindInfo signalBindInfo)
            : base(signalBindInfo)
        {
        }
        public UniReduxSignalCopyBinder WithId(object identifier)
        {
            SignalBindInfo.Identifier = identifier;
            return this;
        }
    }
}
