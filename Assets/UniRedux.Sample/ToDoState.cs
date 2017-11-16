using System;
using UniSystem.Collections.Immutable;

namespace UniRedux.Sample
{
    /// <summary>
    /// ToDo一覧の状態
    /// </summary>
    [Serializable]
    public class ToDoState
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
    /// ToDoフィルタータイプ
    /// </summary>
    public enum TodosFilter
    {
        All = 0,
        InProgress = 1,
        Completed = 2,
    }
}