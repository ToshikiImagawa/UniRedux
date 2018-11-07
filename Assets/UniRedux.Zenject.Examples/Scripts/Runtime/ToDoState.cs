using System;

namespace UniRedux.Zenject.Examples
{
    [Serializable]
    public class ToDoState
    {
        public ToDoFilter Filter;
        public int NextToDoId;
        public ToDo[] ToDos;
    }

    [Serializable]
    public class ToDo
    {
        public int Id;
        public string Text;
        public bool Completed;
        public bool Selected;
    }
}