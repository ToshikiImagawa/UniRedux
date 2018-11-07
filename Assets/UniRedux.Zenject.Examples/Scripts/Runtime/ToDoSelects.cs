using System;
using System.Linq;

namespace UniRedux.Zenject.Examples
{
    public static class ToDoSelects
    {
        public static ToDo[] GetFilterToDos(this ToDoState state)
        {
            var filter = state.Filter;
            switch (filter)
            {
                case ToDoFilter.InProgress:
                    return state.ToDos.Where(todo => !todo.Completed).ToArray();
                case ToDoFilter.Completed:
                    return state.ToDos.Where(todo => todo.Completed).ToArray();
                case ToDoFilter.All:
                    return state.ToDos;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}