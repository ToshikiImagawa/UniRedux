namespace UniRedux.Zenject.Examples
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

            return new ToDoState()
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId,
                ToDos = ToDosReducer.Execute(previousState.ToDos, action)
            };
        }

        private static ToDoState ChangeToDosFilterReducer(ToDoState previousState, ChangeToDoFilterAction action)
        {
            return new ToDoState
            {
                Filter = action.Filter,
                NextToDoId = previousState.NextToDoId,
                ToDos = previousState.ToDos
            };
        }

        private static ToDoState AddToDoReducer(ToDoState previousState, AddToDoAction action)
        {
            return new ToDoState
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId + 1,
                ToDos = previousState.ToDos.AddItem(new ToDo
                {
                    Id = previousState.NextToDoId,
                    Completed = false,
                    Selected = false,
                    Text = action.Text
                })
            };
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static ToDoState InitState => new ToDoState
        {
            NextToDoId = 0,
            Filter = ToDoFilter.All,
            ToDos = ToDosReducer.InitState
        };
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
        public static ToDo[] Execute(ToDo[] previousState, object action)
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

        private static ToDo[] RemoveSelectedTodoReducer(ToDo[] previousState, RemoveSelectedTodoAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Selected);
        }

        private static ToDo[] DeleteToDoReducer(ToDo[] previousState,
            DeleteToDoAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Id == action.ToDoId);
        }

        private static ToDo[] CompleteAllTodoReducer(ToDo[] previousState,
            CompleteAllTodoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = toDo.Selected,
                Completed = action.IsCompleted
            });
        }

        private static ToDo[] CompleteSelectedTodoReducer(ToDo[] previousState,
            CompleteSelectedTodoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = toDo.Selected,
                Completed = toDo.Selected ? action.IsCompleted : toDo.Completed
            });
        }

        private static ToDo[] UpdateSelectedAllToDoReducer(ToDo[] previousState,
            UpdateSelectedAllToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = action.IsSelected,
                Completed = toDo.Completed
            });
        }

        private static ToDo[] ToggleToDoReducer(ToDo[] previousState,
            ToggleCompletedToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = toDo.Selected,
                Completed = !toDo.Completed
            }, toDo => toDo.Id == action.ToDoId);
        }

        private static ToDo[] UpdateTextToDoReducer(ToDo[] previousState,
            UpdateTextToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = action.Text,
                Selected = toDo.Selected,
                Completed = toDo.Completed
            }, toDo => toDo.Id == action.ToDoId);
        }

        private static ToDo[] UpdateSelectedToDoReducer(ToDo[] previousState,
            UpdateSelectedToDoAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = action.IsSelected,
                Completed = toDo.Completed
            }, toDo => toDo.Id == action.ToDoId);
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static ToDo[] InitState => Util.Empty<ToDo>();
    }
}