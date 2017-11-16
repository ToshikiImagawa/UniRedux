using System;
using UniRedux;

namespace UniReduxEditor
{
    [Serializable]
    public class UniReduxTreeElement
    {
        public int StateId;
        public string StateName;
        public string StateTypeName;
        public string StateValue;
        public StateType StateType;
        public int StateDepth;

        public UniReduxTreeElement(int id, string name, string typeName, string value, StateType type, int depth)
        {
            StateId = id;
            StateName = name;
            StateTypeName = typeName;
            StateValue = value;
            StateType = type;
            StateDepth = depth;
        }
    }
}