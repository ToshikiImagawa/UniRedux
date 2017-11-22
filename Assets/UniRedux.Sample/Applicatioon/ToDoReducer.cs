using System.Linq;
using UniSystem.Collections.Immutable;

namespace UniRedux.Sample.Application
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
            if (action.GetType() == typeof(ChangeToDosFilterAction))
            {
                return ChangeToDosFilterReducer(previousState, (ChangeToDosFilterAction)action);
            }
            if (action.GetType() == typeof(AddToDoAction))
            {
                return AddToDoReducer(previousState, (AddToDoAction)action);
            }
            if (action.GetType() == typeof(RemoveSelectedTodosAction))
            {
                return RemoveSelectedTodosReducer(previousState, (RemoveSelectedTodosAction)action);
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
                ToDos = previousState.ToDos.Add(new ToDo
                {
                    Id = previousState.NextToDoId,
                    Completed = false,
                    Selected = false,
                    Text = action.Text
                })
            };
        }

        private static ToDoState RemoveSelectedTodosReducer(ToDoState previousState, RemoveSelectedTodosAction action)
        {
            return new ToDoState
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId + 1,
                ToDos = previousState.ToDos.RemoveAll(toDo => toDo.Selected)
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
        public static ImmutableArray<ToDo> Execute(ImmutableArray<ToDo> previousState, object action)
        {
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

        private static ImmutableArray<ToDo> DeleteToDoReducer(ImmutableArray<ToDo> previousState,
            DeleteToDoAction action)
        {
            return previousState.RemoveAll(toDo => toDo.Id == action.ToDoId);
        }

        private static ImmutableArray<ToDo> CompleteAllTodosReducer(ImmutableArray<ToDo> previousState,
            CompleteAllTodosAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = x.Selected,
                Completed = action.IsCompleted
            }).ToImmutableArray();
        }

        private static ImmutableArray<ToDo> CompleteSelectedTodosReducer(ImmutableArray<ToDo> previousState,
            CompleteSelectedTodosAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = x.Selected,
                Completed = x.Selected ? action.IsCompleted : x.Completed
            }).ToImmutableArray();
        }

        private static ImmutableArray<ToDo> UpdateSelectedAllToDoReducer(ImmutableArray<ToDo> previousState,
            UpdateSelectedAllToDoAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = action.IsSelected,
                Completed = x.Completed
            }).ToImmutableArray();
        }

        private static ImmutableArray<ToDo> ToggleToDoReducer(ImmutableArray<ToDo> previousState,
            ToggleCompletedToDoAction action)
        {
            var todoToEdit = previousState.First(todo => todo.Id == action.ToDoId);

            return previousState.Replace(todoToEdit, new ToDo
            {
                Id = todoToEdit.Id,
                Text = todoToEdit.Text,
                Selected = todoToEdit.Selected,
                Completed = !todoToEdit.Completed
            });
        }

        private static ImmutableArray<ToDo> UpdateTextToDoReducer(ImmutableArray<ToDo> previousState,
            UpdateTextToDoAction action)
        {
            var todoToEdit = previousState.First(todo => todo.Id == action.ToDoId);

            return previousState.Replace(todoToEdit, new ToDo
            {
                Id = todoToEdit.Id,
                Text = action.Text,
                Selected = todoToEdit.Selected,
                Completed = todoToEdit.Completed
            });
        }

        private static ImmutableArray<ToDo> UpdateSelectedToDoReducer(ImmutableArray<ToDo> previousState,
            UpdateSelectedToDoAction action)
        {
            var todoToEdit = previousState.First(todo => todo.Id == action.ToDoId);
            return previousState.Replace(todoToEdit, new ToDo
            {
                Id = todoToEdit.Id,
                Text = todoToEdit.Text,
                Selected = action.IsSelected,
                Completed = todoToEdit.Completed
            });
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static ImmutableArray<ToDo> InitState => Util.Empty<ToDo>().ToImmutableArray();
    }
}