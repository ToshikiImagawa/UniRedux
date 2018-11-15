using System;
using UnityEngine;

namespace UniRedux.Examples.Application
{
    public class ToDoView : ToDoViewBase
    {
        [SerializeField] private ToDoElement _toDoElementPrefab;

        protected override IStore<ToDoState> CurrentStore
            => UniReduxApplication.GetApplication<ToDoState>().CurrentStore;

        protected override Func<Transform, ToDoElementBase> ToDoElementFactory =>
            parent => Instantiate(_toDoElementPrefab, parent, false);
    }
}