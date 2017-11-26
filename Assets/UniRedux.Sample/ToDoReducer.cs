namespace UniRedux.Sample
{
    /// <summary>
    /// ToDoList Reducer
    /// </summary>
    public static class ToDoReducer
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
            if (action.GetType() == typeof(ChangeToDosFilterAction))
            {
                return ChangeToDosFilterReducer(previousState, (ChangeToDosFilterAction)action);
            }
            if (action.GetType() == typeof(AddToDoAction))
            {
                return AddToDoReducer(previousState, (AddToDoAction)action);
            }
            return new ToDoState()
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId,
                ToDos = ToDosReducer.Execute(previousState.ToDos, action)
            };
        }

        private static ToDoState ChangeToDosFilterReducer(ToDoState previousState, ChangeToDosFilterAction action)
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
            Filter = TodosFilter.All,
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
            if (action.GetType() == typeof(RemoveSelectedTodosAction))
            {
                return RemoveSelectedTodosReducer(previousState, (RemoveSelectedTodosAction)action);
            }
            if (action.GetType() == typeof(DeleteToDoAction))
            {
                return DeleteToDoReducer(previousState, (DeleteToDoAction)action);
            }
            if (action.GetType() == typeof(CompleteSelectedTodosAction))
            {
                return CompleteSelectedTodosReducer(previousState, (CompleteSelectedTodosAction)action);
            }
            if (action.GetType() == typeof(CompleteAllTodosAction))
            {
                return CompleteAllTodosReducer(previousState, (CompleteAllTodosAction)action);
            }
            if (action.GetType() == typeof(ToggleCompletedToDoAction))
            {
                return ToggleToDoReducer(previousState, (ToggleCompletedToDoAction)action);
            }
            if (action.GetType() == typeof(UpdateTextToDoAction))
            {
                return UpdateTextToDoReducer(previousState, (UpdateTextToDoAction)action);
            }
            if (action.GetType() == typeof(UpdateSelectedToDoAction))
            {
                return UpdateSelectedToDoReducer(previousState, (UpdateSelectedToDoAction)action);
            }
            if (action.GetType() == typeof(UpdateSelectedAllToDoAction))
            {
                return UpdateSelectedAllToDoReducer(previousState, (UpdateSelectedAllToDoAction)action);
            }

            return previousState;
        }

        private static ToDo[] RemoveSelectedTodosReducer(ToDo[] previousState, RemoveSelectedTodosAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Selected);
        }

        private static ToDo[] DeleteToDoReducer(ToDo[] previousState,
            DeleteToDoAction action)
        {
            return previousState.RemoveItem(toDo => toDo.Id == action.ToDoId);
        }

        private static ToDo[] CompleteAllTodosReducer(ToDo[] previousState,
            CompleteAllTodosAction action)
        {
            return previousState.UpdateItem(toDo => new ToDo
            {
                Id = toDo.Id,
                Text = toDo.Text,
                Selected = toDo.Selected,
                Completed = action.IsCompleted
            });
        }

        private static ToDo[] CompleteSelectedTodosReducer(ToDo[] previousState,
            CompleteSelectedTodosAction action)
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