using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniRedux.Sample;

namespace UniRedux
{
#if UNITY_EDITOR
    public static class UniDebug
    {
        public static string ToJson<TState>(this TState state) where TState : class
        {
            return ToJson(StateReflection.Serialize(state));
        }

        public static string ToJson(this SerializableStateElement element)
        {
            var childrenText = element.Children == null
                ? ""
                : string.Join(",", element.Children.Select(ToJson).ToArray());

            return "{" +
                   $"\"Name\" : \"{element.Name}\", \"Value\" : \"{element.Value}\", \"Children\" : [{childrenText}], \"TypeName\" : \"{element.Type}\", \"Type\" : \"{element.ObjectType}\"" +
                   "}";
        }

        public static SerializableStateElement ToSerialize<TState>(this TState state, bool isProperty = true) where TState : class
        {
            return StateReflection.Serialize(state, isProperty);
        }
    }

    public static class StateReflection
    {
        public static SerializableStateElement Serialize<TState>(TState state, bool isProperty = true)
        {
            return new SerializableStateElement
            {
                Name = $"{state.GetType().Name}State",
                Value = "",
                Type = state.GetType(),
                ObjectType = ObjectType.Object,
                Children = GetChildren(state, isProperty)
            };
        }

        private static SerializableStateElement[] GetChildren(object obj, bool isProperty)
        {
            if (obj == null) return Util.Empty<SerializableStateElement>();
            var stateElementList = new List<SerializableStateElement>();

            var targetType = obj.GetType();
            const BindingFlags allMemberBindingFlags =
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            if (isProperty)
            {
                var targetProperties = targetType.GetProperties(allMemberBindingFlags);
                foreach (var propertyInfo in targetProperties)
                {
                    var name = propertyInfo.Name;
                    var value = propertyInfo.GetValue(obj);
                    var type = propertyInfo.PropertyType;

                    AddSerializableStateElement(name, value, type, stateElementList.Add, true);
                }
            }
            else
            {
                var targetFields = targetType.GetFields(allMemberBindingFlags);
                foreach (var targetField in targetFields)
                {
                    var name = targetField.Name;
                    var value = targetField.GetValue(obj);
                    var type = targetField.FieldType;

                    AddSerializableStateElement(name, value, type, stateElementList.Add, false);
                }
            }

            return stateElementList.ToArray();
        }

        private static SerializableStateElement[] GetChildren(IEnumerable values, ref Type collectionType, bool isProperty)
        {
            if (values == null) return Util.Empty<SerializableStateElement>();
            var stateElementList = new List<SerializableStateElement>();
            var index = 0;
            foreach (var value in values)
            {
                if (index == 0) collectionType = value.GetType();

                var isValueType = collectionType.IsValueType || value is string;
                var isArray = /*value.GetType().IsArray ||*/ value is IEnumerable;
                if (value is string) isArray = false;

                if (isArray)
                {
                    var childrenCollectionType = collectionType;
                    var children = GetChildren(value as IEnumerable, ref childrenCollectionType, isProperty);
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = $"{childrenCollectionType.Name}[]",
                        Type = collectionType,
                        ObjectType = ObjectType.Array,
                        Children = children
                    });
                }
                else if (!isValueType)
                {
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = "",
                        Type = collectionType,
                        ObjectType = ObjectType.Object,
                        Children = GetChildren(value, isProperty)
                    });
                }
                else
                {
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = value,
                        Type = collectionType,
                        ObjectType = ObjectType.Value,
                        Children = Util.Empty<SerializableStateElement>()
                    });
                }
                index++;
            }
            return stateElementList.ToArray();
        }

        private static void AddSerializableStateElement(string name, object value,Type type,
            Action<SerializableStateElement> listAddAction,
            bool isProperty)
        {
            var isValueType = type.IsValueType || value is string;
            var isArray = /*value.GetType().IsArray ||*/ value is IEnumerable;
            if (value is string) isArray = false;

            var element = new SerializableStateElement
            {
                Name = name,
                Value = isArray ? $"{type.Name}[]" : isValueType ? value : "",
                Type = type,
                ObjectType = isArray ? ObjectType.Array : isValueType ? ObjectType.Value : ObjectType.Object,
                Children = Util.Empty<SerializableStateElement>()
            };

            if (isArray)
            {
                Type collectionType = type;
                element.Children = GetChildren(value as IEnumerable,ref collectionType, isProperty);
                element.Value = $"{collectionType.Name}[]";
            }
            else if (!isValueType)
            {
                element.Children = GetChildren(value, isProperty);
            }

            listAddAction(element);
        }
    }

    [Serializable]
    public class SerializableStateElement
    {
        public string Name;
        public object Value;
        public SerializableStateElement[] Children;
        public Type Type;
        public ObjectType ObjectType;
    }

    public enum ObjectType
    {
        Value = 0,
        Array = 1,
        Object = 2
    }
#endif
}