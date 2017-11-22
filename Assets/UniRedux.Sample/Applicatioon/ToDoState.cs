using System;
using UniSystem.Collections.Immutable;

namespace UniRedux.Sample.Application
{
    /// <summary>
    /// ToDo一覧の状態
    /// </summary>
    [Serializable]
    public struct ToDoState
    {
        public TodosFilter Filter { get; set; }
        public int NextToDoId { get; set; }
        public ImmutableArray<ToDo> ToDos { get; set; }

#if UNITY_EDITOR
        public override string ToString()
        {
            return this.ToJson();
        }
#endif
    }
    /// <summary>
    /// ToDoの状態
    /// </summary>
    [Serializable]
    public struct ToDo
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool Completed { get; set; }
        public bool Selected { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is ToDo)) return false;
            var toDo = (ToDo)obj;
            return toDo.Id == Id && toDo.Text == Text && toDo.Completed == Completed && toDo.Selected == Selected;
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
    /// <summary>
    /// ToDoフィルタータイプ
    /// </summary>
    public enum TodosFilter
    {
        All = 0,
        InProgress = 1,
        Completed = 2,
    }
}