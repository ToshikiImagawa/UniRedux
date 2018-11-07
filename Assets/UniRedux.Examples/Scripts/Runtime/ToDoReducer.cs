using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Examples
{
    /// <summary>
    /// ToDoList Reducer
    /// </summary>
    internal static class ToDoReducer
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static ToDoState Execute(ToDoState previousState, object action)
        {
            if (previousState == null) previousState = InitState;
            if (action is ChangeToDoFilterAction)
            {
                return ChangeToDosFilterReducer(previousState, (ChangeToDoFilterAction) action);
            }

            if (action is AddToDoAction)
            {
                return AddToDoReducer(previousState, (AddToDoAction) action);
            }

            return new ToDoState
            (
                previousState.Filter,
                previousState.NextToDoId,
                ToDosReducer.Execute(previousState.ToDos, action)
            );
        }

        private static ToDoState ChangeToDosFilterReducer(ToDoState previousState, ChangeToDoFilterAction action)
        {
            return new ToDoState
            (
                action.Filter,
                previousState.NextToDoId,
                previousState.ToDos
            );
        }

        private static ToDoState AddToDoReducer(ToDoState previousState, AddToDoAction action)
        {
            return new ToDoState
            (
                previousState.Filter,
                previousState.NextToDoId + 1,
                previousState.ToDos.AddItem(
                    previousState.NextToDoId,
                    new ToDo
                    (
                        previousState.NextToDoId,
                        action.Text,
                        false,
                        false
                    )
                ).ToDictionary(todo => todo.Key, todo => todo.Value)
            );
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static ToDoState InitState => new ToDoState
        (
            ToDoFilter.All,
            0,
            ToDosReducer.InitState
        );
    }

    /// <summary>
    /// ToDos Reducer
    /// </summary>
    public static class ToDosReducer
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="previousState"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Dictionary<int, ToDo> Execute(Dictionary<int, ToDo> previousState, object action)
        {
            if (action is RemoveSelectedTodoAction)
            {
                return RemoveSelectedTodoReducer(previousState, (RemoveSelectedTodoAction) action);
            }

            if (action is DeleteToDoAction)
            {
                return DeleteToDoReducer(previousState, (DeleteToDoAction) action);
            }

            if (action is CompleteSelectedTodoAction)
            {
                return CompleteSelectedTodoReducer(previousState, (CompleteSelectedTodoAction) action);
            }

            if (action is CompleteAllTodoAction)
            {
                return CompleteAllTodoReducer(previousState, (CompleteAllTodoAction) action);
            }

            if (action is ToggleCompletedToDoAction)
            {
                return ToggleToDoReducer(previousState, (ToggleCompletedToDoAction) action);
            }

            if (action is UpdateTextToDoAction)
            {
                return UpdateTextToDoReducer(previousState, (UpdateTextToDoAction) action);
            }

            if (action is UpdateSelectedToDoAction)
            {
                return UpdateSelectedToDoReducer(previousState, (UpdateSelectedToDoAction) action);
            }

            if (action is UpdateSelectedAllToDoAction)
            {
                return UpdateSelectedAllToDoReducer(previousState, (UpdateSelectedAllToDoAction) action);
            }

            return previousState;
        }

        private static Dictionary<int, ToDo> RemoveSelectedTodoReducer(IDictionary<int, ToDo> previousState,
            RemoveSelectedTodoAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Selected).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> DeleteToDoReducer(IDictionary<int, ToDo> previousState,
            DeleteToDoAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Id == action.ToDoId)
                .ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> CompleteAllTodoReducer(IDictionary<int, ToDo> previousState,
            CompleteAllTodoAction action)
        {
            return previousState.UpdateItem<int, ToDo>(toDo => new ToDo
            (
                toDo.Id,
                toDo.Text,
                toDo.Selected,
                action.IsCompleted
            )).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> CompleteSelectedTodoReducer(IDictionary<int, ToDo> previousState,
            CompleteSelectedTodoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            (
                toDo.Id,
                toDo.Text,
                toDo.Selected,
                toDo.Selected ? action.IsCompleted : toDo.Completed
            )).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> UpdateSelectedAllToDoReducer(IDictionary<int, ToDo> previousState,
            UpdateSelectedAllToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            (
                toDo.Id,
                toDo.Text,
                action.IsSelected,
                toDo.Completed
            )).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> ToggleToDoReducer(IDictionary<int, ToDo> previousState,
            ToggleCompletedToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            (
                toDo.Id,
                toDo.Text,
                toDo.Selected,
                !toDo.Completed
            ), toDo => toDo.Id == action.ToDoId).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> UpdateTextToDoReducer(IDictionary<int, ToDo> previousState,
            UpdateTextToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            (
                toDo.Id,
                action.Text,
                toDo.Selected,
                toDo.Completed
            ), toDo => toDo.Id == action.ToDoId).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        private static Dictionary<int, ToDo> UpdateSelectedToDoReducer(IDictionary<int, ToDo> previousState,
            UpdateSelectedToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            (
                toDo.Id,
                toDo.Text,
                action.IsSelected,
                toDo.Completed
            ), toDo => toDo.Id == action.ToDoId).ToDictionary(todo => todo.Key, todo => todo.Value);
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static Dictionary<int, ToDo> InitState => new Dictionary<int, ToDo>();
    }
}