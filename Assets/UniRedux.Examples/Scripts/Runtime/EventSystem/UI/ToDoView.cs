using System;
using UnityEngine;

namespace UniRedux.Examples.EventSystem
{
    public class ToDoView : ToDoViewBase
    {
        [SerializeField] private ToDoElement _toDoElementPrefab;

        protected override IStore<ToDoState> CurrentStore
            => ToDoApp.Store;

        protected override Func<Transform, ToDoElementBase> ToDoElementFactory =>
            parent => Instantiate(_toDoElementPrefab, parent, false);
    }
}