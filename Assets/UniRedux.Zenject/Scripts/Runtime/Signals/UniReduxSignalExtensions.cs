using System;
using System.Collections.Generic;
using Zenject;

namespace UniRedux
{
    public static class UniReduxSignalExtensions
    {
        public static UniReduxSignalParentIdCopyBinder DeclareUniReduxSignal<TLocalState, TOriginalState>(this DiContainer container, Func<TOriginalState, TLocalState> converter, IEqualityComparer<TLocalState> equalityComparer = null)
        {
            var signalBindInfo = new UniReduxSignalDeclarationBindInfo(typeof(TLocalState), new UniReduxStateDirector<TLocalState, TOriginalState>(converter, equalityComparer));

            var bindInfo = container.Bind<UniReduxSignalDeclaration>().AsCached()
                .WithArguments(signalBindInfo).WhenInjectedInto<UniReduxSignalBus>().BindInfo;
            var signalBinder = new UniReduxSignalParentIdCopyBinder(signalBindInfo);
            signalBinder.AddCopyBindInfo(bindInfo);
            return signalBinder;
        }
        public static BindUniReduxSignalParentIdToBinder<TLocalState> BindUniReduxSignal<TLocalState>(this DiContainer container)
        {
            var signalBindInfo = new UniReduxSignalBindingBindInfo(typeof(TLocalState));

            return new BindUniReduxSignalParentIdToBinder<TLocalState>(container, signalBindInfo);
        }
    }
}
