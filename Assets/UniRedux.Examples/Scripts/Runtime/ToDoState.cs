using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Examples
{
    [Serializable]
    public class ToDoState : IEquatable<ToDoState>
    {
        public readonly ToDoFilter Filter;
        public readonly int NextToDoId;
        public readonly Dictionary<int, ToDo> ToDos;

        public ToDoState(ToDoFilter filter, int nextToDoId, Dictionary<int, ToDo> toDos)
        {
            Filter = filter;
            NextToDoId = nextToDoId;
            ToDos = toDos;
        }

        public bool Equals(ToDoState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Filter == other.Filter && NextToDoId == other.NextToDoId && Equals(ToDos, other.ToDos);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ToDoState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Filter;
                hashCode = (hashCode * 397) ^ NextToDoId;
                hashCode = (hashCode * 397) ^ (ToDos != null ? ToDos.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    [Serializable]
    public class ToDo : IEquatable<ToDo>
    {
        public readonly int Id;
        public readonly string Text;
        public readonly bool Completed;
        public readonly bool Selected;

        public ToDo(int id, string text, bool selected, bool completed)
        {
            Id = id;
            Text = text;
            Selected = selected;
            Completed = completed;
        }

        public ToDo()
        {
            Id = 0;
            Text = string.Empty;
            Completed = false;
            Selected = false;
        }

        public bool Equals(ToDo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && string.Equals(Text, other.Text) && Completed == other.Completed &&
                   Selected == other.Selected;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ToDo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Completed.GetHashCode();
                hashCode = (hashCode * 397) ^ Selected.GetHashCode();
                return hashCode;
            }
        }
    }
}