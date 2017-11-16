namespace UniRedux.Sample
{
    /// <summary>
    /// Action to add todo
    /// </summary>
    public struct AddToDoAction
    {
        public string Text { get; set; }
    }

    /// <summary>
    /// Action to delete todo
    /// </summary>
    public struct DeleteToDoAction
    {
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to update todo text
    /// </summary>
    public struct UpdateTextToDoAction
    {
        public string Text { get; set; }
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to toggle completed todo flag
    /// </summary>
    public struct ToggleCompletedToDoAction
    {
        public int ToDoId { get; set; }
    }

    /// <summary>
    /// Action to complete all todo
    /// </summary>
    public class CompleteAllTodosAction
    {
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Action to complete all todo
    /// </summary>
    public class CompleteSelectedTodosAction
    {
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Action to toggle completed todo flag
    /// </summary>
    public struct UpdateSelectedToDoAction
    {
        public int ToDoId { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Action to change todos filter
    /// </summary>
    public struct ChangeToDosFilterAction
    {
        public TodosFilter Filter { get; set; }
    }
}