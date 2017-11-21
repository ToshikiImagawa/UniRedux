using System;
using System.Linq;
using UniRedux.Sample;

namespace UniRedux.Sample2
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
                return ChangeToDosFilterReducer(previousState, (ChangeToDosFilterAction) action);
            }
            if (action.GetType() == typeof(AddToDoAction))
            {
                return AddToDoReducer(previousState, (AddToDoAction) action);
            }
            if (action.GetType() == typeof(RemoveSelectedTodosAction))
            {
                return RemoveSelectedTodosReducer(previousState, (RemoveSelectedTodosAction) action);
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
            var toDoList = previousState.ToDos.ToList();
            toDoList.Add(new ToDo
            {
                Id = previousState.NextToDoId,
                Completed = false,
                Selected = false,
                Text = action.Text
            });
            return new ToDoState
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId + 1,
                ToDos = toDoList.ToArray()
            };
        }

        private static ToDoState RemoveSelectedTodosReducer(ToDoState previousState, RemoveSelectedTodosAction action)
        {
            var toDoList = previousState.ToDos.ToList();
            toDoList.RemoveAll(toDo => toDo.Selected);
            return new ToDoState
            {
                Filter = previousState.Filter,
                NextToDoId = previousState.NextToDoId + 1,
                ToDos = toDoList.ToArray()
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
            if (action.GetType() == typeof(DeleteToDoAction))
            {
                return DeleteToDoReducer(previousState, (DeleteToDoAction) action);
            }
            if (action.GetType() == typeof(CompleteSelectedTodosAction))
            {
                return CompleteSelectedTodosReducer(previousState, (CompleteSelectedTodosAction) action);
            }
            if (action.GetType() == typeof(CompleteAllTodosAction))
            {
                return CompleteAllTodosReducer(previousState, (CompleteAllTodosAction) action);
            }
            if (action.GetType() == typeof(ToggleCompletedToDoAction))
            {
                return ToggleToDoReducer(previousState, (ToggleCompletedToDoAction) action);
            }
            if (action.GetType() == typeof(UpdateTextToDoAction))
            {
                return UpdateTextToDoReducer(previousState, (UpdateTextToDoAction) action);
            }
            if (action.GetType() == typeof(UpdateSelectedToDoAction))
            {
                return UpdateSelectedToDoReducer(previousState, (UpdateSelectedToDoAction) action);
            }
            if (action.GetType() == typeof(UpdateSelectedAllToDoAction))
            {
                return UpdateSelectedAllToDoReducer(previousState, (UpdateSelectedAllToDoAction) action);
            }

            return previousState;
        }

        private static ToDo[] DeleteToDoReducer(ToDo[] previousState,
            DeleteToDoAction action)
        {
            var previousStateList = previousState.ToList();
            previousStateList.RemoveAll(toDo => toDo.Id == action.ToDoId);
            return previousStateList.ToArray();
        }

        private static ToDo[] CompleteAllTodosReducer(ToDo[] previousState,
            CompleteAllTodosAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = x.Selected,
                Completed = action.IsCompleted
            }).ToArray();
        }

        private static ToDo[] CompleteSelectedTodosReducer(ToDo[] previousState,
            CompleteSelectedTodosAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = x.Selected,
                Completed = x.Selected ? action.IsCompleted : x.Completed
            }).ToArray();
        }

        private static ToDo[] UpdateSelectedAllToDoReducer(ToDo[] previousState,
            UpdateSelectedAllToDoAction action)
        {
            return previousState.Select(x => new ToDo
            {
                Id = x.Id,
                Text = x.Text,
                Selected = action.IsSelected,
                Completed = x.Completed
            }).ToArray();
        }

        private static ToDo[] ToggleToDoReducer(ToDo[] previousState,
            ToggleCompletedToDoAction action)
        {
            var index = Array.IndexOf(previousState.Select(item => item.Id).ToArray(), action.ToDoId);
            var todoToEdit = previousState[index];
            if (index > -1)
            {
                previousState[index] = new ToDo
                {
                    Id = todoToEdit.Id,
                    Text = todoToEdit.Text,
                    Selected = todoToEdit.Selected,
                    Completed = !todoToEdit.Completed
                };
            }
            return previousState;
        }

        private static ToDo[] UpdateTextToDoReducer(ToDo[] previousState,
            UpdateTextToDoAction action)
        {
            var index = Array.IndexOf(previousState.Select(item => item.Id).ToArray(), action.ToDoId);
            var todoToEdit = previousState[index];
            if (index > -1)
            {
                previousState[index] = new ToDo
                {
                    Id = todoToEdit.Id,
                    Text = action.Text,
                    Selected = todoToEdit.Selected,
                    Completed = todoToEdit.Completed
                };
            }
            return previousState;
        }

        private static ToDo[] UpdateSelectedToDoReducer(ToDo[] previousState,
            UpdateSelectedToDoAction action)
        {
            var index = Array.IndexOf(previousState.Select(item => item.Id).ToArray(), action.ToDoId);
            var todoToEdit = previousState[index];
            if (index > -1)
            {
                previousState[index] = new ToDo
                {
                    Id = todoToEdit.Id,
                    Text = todoToEdit.Text,
                    Selected = action.IsSelected,
                    Completed = todoToEdit.Completed
                };
            }
            return previousState;
        }

        /// <summary>
        /// Initial value of State
        /// </summary>
        public static ToDo[] InitState => Util.Empty<ToDo>();
    }
}