using System;
using System.Linq;

namespace UniRedux.Sample
{
    public static class ToDoSelects
    {
        public static ToDo[] GetFilterToDos(this ToDoState state)
        {
            var filter = state.Filter;
            switch (filter)
            {
                case TodosFilter.InProgress:
                    return state.ToDos.Where(todo => !todo.Completed).ToArray();
                case TodosFilter.Completed:
                    return state.ToDos.Where(todo => todo.Completed).ToArray();
                case TodosFilter.All:
                    return state.ToDos;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}