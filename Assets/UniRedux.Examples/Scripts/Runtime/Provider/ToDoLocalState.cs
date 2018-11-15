using System;
using System.Collections.Generic;
using System.Linq;
using UniRedux.Examples;

namespace UniRedux.Provider.Examples
{
    public class ToDoLocalState : IEquatable<ToDoLocalState>
    {
        public ToDoFilter Filter { get; set; }
        public Dictionary<int, ToDo> ToDos { get; set; }

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
            return Equals((ToDoLocalState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Filter * 397) ^ (ToDos != null ? ToDos.GetHashCode() : 0);
            }
        }
    }
}