﻿using System;

namespace UniRedux.Sample
{
    [Serializable]
    public class ToDoState
    {
        public TodosFilter Filter;
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