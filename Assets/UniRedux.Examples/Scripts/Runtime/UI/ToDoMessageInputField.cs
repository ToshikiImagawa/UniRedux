using UnityEngine.UI;

namespace UniRedux.Examples
{
    public class ToDoMessageInputField : InputField, IToDoMessageInputModule
    {
        public string ToDoMessage
        {
            get { return text; }
            set { text = value; }
        }
    }
}