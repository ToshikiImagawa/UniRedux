using System;
using System.Linq;

namespace UniRedux.Zenject.Examples.Signal
{
    public class ToDoLocalState : IEquatable<ToDoLocalState>
    {
        public ToDoFilter Filter { get; set; }
        public ToDo[] ToDos { get; set; }

        public ToDo[] FilterToDos
        {
            get
            {
                switch (Filter)
                {
                    case ToDoFilter.All:
                        return ToDos;
                    case ToDoFilter.InProgress:
                        return ToDos.Where(todo => !todo.Completed).ToArray();
                    case ToDoFilter.Completed:
                        return ToDos.Where(todo => todo.Completed).ToArray();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public bool Equals(ToDoLocalState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Filter == other.Filter && other.ToDos.SequenceEqual(ToDos);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            if (obj.GetType() != GetType()) return false;
            return Equals((ToDoLocalState)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Filter * 397) ^ (ToDos != null ? ToDos.GetHashCode() : 0);
            }
        }
    }
}
