using System.Collections.Generic;
using System.Linq;

namespace UniRedux.Sample
{
    public static class ToDoListReducer
    {
        public static ToDoListState Execute(ToDoListState previousState, object action)
        {
            var toDoList = previousState.ToDoList?.ToList() ?? new List<ToDoState>();
            # region AddToDoAction
            if (action.GetType() == typeof(AddToDoAction))
            {
                var addToDoAction = (AddToDoAction)action;
                var toDoState = new ToDoState
                {
                    ToDoId = previousState.NextToDoId,
                    Text = addToDoAction.Text,
                    Completed = false
                };
                toDoList.Add(toDoState);
                previousState.NextToDoId += 1;
                return new ToDoListState
                {
                    NextToDoId = previousState.NextToDoId++,
                    ToDoList = toDoList.ToArray()
                };
            }
            #endregion

            #region DeleteToDoAction
            if (action.GetType() == typeof(DeleteToDoAction))
            {
                var deleteToDoAction = (DeleteToDoAction)action;
                var hitToDoList = toDoList.Where(toDo => toDo.ToDoId == deleteToDoAction.ToDoId).ToArray();
                foreach (var hitToDo in hitToDoList)
                {
                    toDoList.Remove(hitToDo);
                }
                return new ToDoListState
                {
                    NextToDoId = previousState.NextToDoId,
                    ToDoList = toDoList.ToArray()
                };
            }
            # endregion
            for (var i = 0; i < toDoList.Count; i++)
            {
                previousState.ToDoList[i] = ToDoStateReducer.Execute(previousState.ToDoList[i], action);
            }
            previousState.ToDoList = toDoList.ToArray();
            return previousState;
        }
        public static ToDoListState InitState => new ToDoListState
        {
            NextToDoId = 0,
            ToDoList = Util.Empty<ToDoState>()
        };
    }

    public static class ToDoStateReducer
    {
        public static ToDoState Execute(ToDoState previousState, object action)
        {
            if (action.GetType() == typeof(UpdateTextToDoAction))
            {
                var updateTextToDoAction = (UpdateTextToDoAction)action;
                if (previousState.ToDoId != updateTextToDoAction.ToDoId) return previousState;
                previousState.Text = updateTextToDoAction.Text;
                return previousState;
            }
            if (action.GetType() == typeof(ToggleToDoAction))
            {
                var toggleToDoAction = (ToggleToDoAction)action;
                if (previousState.ToDoId != toggleToDoAction.ToDoId) return previousState;
                previousState.Completed = !previousState.Completed;
                return previousState;
            }
            return previousState;
        }
        public static ToDoState InitState => new ToDoState
        {
            ToDoId = 0,
            Text = string.Empty,
            Completed = false
        };
    }

    public struct AddToDoAction
    {
        public string Text { get; set; }
    }
    public struct DeleteToDoAction
    {
        public int ToDoId { get; set; }
    }
    public struct UpdateTextToDoAction
    {
        public string Text { get; set; }
        public int ToDoId { get; set; }
    }
    public struct ToggleToDoAction
    {
        public int ToDoId { get; set; }
    }
}