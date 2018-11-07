using System;

namespace UniRedux.Editor
{
    [Serializable]
    public class UniReduxTreeElement
    {
        public int StateId;
        public string StateName;
        public Type StateType;
        public object StateValue;
        public ObjectType ObjectType;
        public int StateDepth;

        public UniReduxTreeElement(int id, string name, Type type, object value, ObjectType objectType, int depth)
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