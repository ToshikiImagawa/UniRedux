using System;
using UniRedux;

namespace UniReduxEditor
{
    [Serializable]
    public class UniReduxTreeElement
    {
        public int StateId;
        public string StateName;
        public Type StateType;
        public string StateValue;
        public ObjectType ObjectType;
        public int StateDepth;

        public UniReduxTreeElement(int id, string name, Type type, string value, ObjectType objectType, int depth)
        {
            StateId = id;
            StateName = name;
            StateType = type;
            StateValue = value;
            ObjectType = objectType;
            StateDepth = depth;
        }
    }
}