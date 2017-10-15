using System;
namespace UniRedux.Sample
{
    public struct ToDoListState
    {
        public int NextToDoId { get; set; }
        public ToDoState[] ToDoList { get; set; }
    }

    public struct ToDoState
    {
        public int ToDoId { get; set; }
        public bool Completed { get; set; }
        public string Text { get; set; }
    }
}