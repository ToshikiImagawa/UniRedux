﻿using UnityEngine;
using Zenject;

namespace UniRedux.Zenject.Examples.Signal.Installers
{
    public class SignalToDoInstaller : MonoInstaller
    {public override void InstallBindings()
        {
            UniReduxSignalBusInstaller.Install(Container);
            Container.Bind<IStore>().FromResolveGetter<ToDoApp>(app => app.Store).AsSingle();
            Container.DeclareUniReduxSignal<ToDoFilter, ToDoState>(state => state.Filter);
            Container.DeclareUniReduxSignal<ToDo[], ToDoState>(state => state.ToDos);

            Container.Bind<ToDoApp>().AsSingle().NonLazy();
        }
    }
}