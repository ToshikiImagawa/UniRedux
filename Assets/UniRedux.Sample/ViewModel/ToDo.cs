using System;

namespace UniRedux.Sample
{
    /// <summary>
    /// ToDoの状態
    /// </summary>
    [Serializable]
    public class ToDo
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
        public bool Selected { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ToDo)) return false;
            var toDo = (ToDo) obj;
            return toDo.Id == Id && toDo.Text == Text && toDo.Completed == Completed && toDo.Selected == Selected;
        }

        protected bool Equals(ToDo other)
        {
            return Id == other.Id && string.Equals(Text, other.Text) && Completed == other.Completed && Selected == other.Selected;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id;
                hashCode = (hashCode * 397) ^ (Text?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Completed.GetHashCode();
                hashCode = (hashCode * 397) ^ Selected.GetHashCode();
                return hashCode;
            }
        }
    }
}