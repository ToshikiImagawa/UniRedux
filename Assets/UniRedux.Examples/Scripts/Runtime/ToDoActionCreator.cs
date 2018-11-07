namespace UniRedux.Examples
{
    public static class ToDoActionCreator
    {
        public static object AddToDo(string text)
        {
            return new AddToDoAction
            {
                Text = text
            };
        }

        public static object DeleteToDo(int toDoId)
        {
            return new DeleteToDoAction
            {
                ToDoId = toDoId
            };
        }

        public static object UpdateTextToDo(string text, int toDoId)
        {
            return new UpdateTextToDoAction
            {
                Text = text,
                ToDoId = toDoId
            };
        }

        public static object ToggleCompletedToDo(int toDoId)
        {
            return new ToggleCompletedToDoAction
            {
                ToDoId = toDoId
            };
        }

        public static object CompleteAllTodo(bool isCompleted)
        {
            return new CompleteAllTodoAction
            {
                IsCompleted = isCompleted
            };
        }

        public static object CompleteSelectedTodo(bool isCompleted)
        {
            return new CompleteSelectedTodoAction
            {
                IsCompleted = isCompleted
            };
        }

        public static object UpdateSelectedToDo(int toDoId, bool isSelected)
        {
            return new UpdateSelectedToDoAction
            {
                ToDoId = toDoId,
                IsSelected = isSelected
            };
        }

        public static object ChangeToDoFilter(ToDoFilter filter)
        {
            return new ChangeToDoFilterAction
            {
                Filter = filter
            };
        }

        public static object UpdateSelectedAllToDo(bool isSelected)
        {
            return new UpdateSelectedAllToDoAction
            {
                IsSelected = isSelected
            };
        }

        public static object RemoveSelectedTodo()
        {
            return new RemoveSelectedTodoAction();
        }
    }

    /// <summary>
    /// Action to add todo
    /// </summary>
    internal struct AddToDoAction
    {
        public string Text { get; set; }
    }

    /// <summary>
    /// Action to delete todo
    /// </summary>
    internal struct DeleteToDoAction
    {
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to update todo text
    /// </summary>
    internal struct UpdateTextToDoAction
    {
        public string Text { get; set; }
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to toggle completed todo flag
    /// </summary>
    internal struct ToggleCompletedToDoAction
    {
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to complete all todo
    /// </summary>
    internal struct CompleteAllTodoAction
    {
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Action to complete all todo
    /// </summary>
    internal struct CompleteSelectedTodoAction
    {
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Action to toggle completed todo flag
    /// </summary>
    internal struct UpdateSelectedToDoAction
    {
        public int ToDoId { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Action to change todo filter
    /// </summary>
    internal struct ChangeToDoFilterAction
    {
        public ToDoFilter Filter { get; set; }
    }

    /// <summary>
    /// Action to toggle completed todo flag
    /// </summary>
    internal struct UpdateSelectedAllToDoAction
    {
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Remove todo
    /// </summary>
    internal struct RemoveSelectedTodoAction
    {
    }
}