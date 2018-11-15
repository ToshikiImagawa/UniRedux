using System;
using UniRedux.Examples;

namespace UniRedux.Provider.Examples
{
    public class ToDoActionDispatcher
    {
        private Dispatcher Dispatcher { get; }

        public Action<string> AddToDo
        {
            get { return text => { Dispatcher(ToDoActionCreator.AddToDo(text)); }; }
        }

        public Action<int> ToggleCompletedToDo
        {
            get { return toDoId => { Dispatcher(ToDoActionCreator.ToggleCompletedToDo(toDoId)); }; }
        }

        public Action<int, bool> UpdateSelectedToDo
        {
            get
            {
                return (toDoId, isSelected) =>
                {
                    Dispatcher(ToDoActionCreator.UpdateSelectedToDo(toDoId, isSelected));
                };
            }
        }

        public Action<bool> UpdateSelectedAllToDo
        {
            get { return isSelected => { Dispatcher(ToDoActionCreator.UpdateSelectedAllToDo(isSelected)); }; }
        }

        public Action RemoveSelectedTodo
        {
            get { return () => { Dispatcher(ToDoActionCreator.RemoveSelectedTodo()); }; }
        }

        public Action<bool> CompleteSelectedTodo
        {
            get { return isCompleted => { Dispatcher(ToDoActionCreator.CompleteSelectedTodo(isCompleted)); }; }
        }

        public ToDoActionDispatcher(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
    }
}