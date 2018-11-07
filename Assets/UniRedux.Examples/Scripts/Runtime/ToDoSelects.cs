using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Examples
{
    public static class ToDoSelects
    {
        public static IEnumerable<ToDo> GetFilterToDos(this Dictionary<int, ToDo> toDos, ToDoFilter filter)
        {
            switch (filter)
            {
                case ToDoFilter.InProgress:
                    return toDos.Where(todo => !todo.Value.Completed).Select(todo => todo.Value).ToArray();
                case ToDoFilter.Completed:
                    return toDos.Where(todo => todo.Value.Completed).Select(todo => todo.Value).ToArray();
                case ToDoFilter.All:
                    return toDos.Select(todo => todo.Value).ToArray();
                default:
                    throw Assert.CreateException();
            }
        }
    }
}