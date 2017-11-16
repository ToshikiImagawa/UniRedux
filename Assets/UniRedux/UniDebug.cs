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
                   $"\"Name\" : \"{element.Name}\", \"Value\" : \"{element.Value}\", \"Children\" : [{childrenText}], \"TypeName\" : \"{element.TypeName}\", \"Type\" : \"{element.Type}\"" +
                   "}";
        }

        public static SerializableStateElement ToSerialize<TState>(this TState state) where TState : class
        {
            return StateReflection.Serialize(state);
        }
    }

    public static class StateReflection
    {
        public static SerializableStateElement Serialize<TState>(TState state)
        {
            return new SerializableStateElement
            {
                Name = $"{state.GetType().Name}State",
                Value = "",
                TypeName = state.GetType().FullName,
                Type = StateType.Object,
                Children = GetChildren(state)
            };
        }

        private static SerializableStateElement[] GetChildren(object obj, bool isProperty = true)
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

                    AddSerializableStateElement(name, value, stateElementList.Add, true);
                }
            }
            else
            {
                var targetFields = targetType.GetFields(allMemberBindingFlags);
                foreach (var targetField in targetFields)
                {
                    var name = targetField.Name;
                    var value = targetField.GetValue(obj);

                    AddSerializableStateElement(name, value, stateElementList.Add, false);
                }
            }

            return stateElementList.ToArray();
        }

        private static SerializableStateElement[] GetChildren(IEnumerable values, bool isProperty)
        {
            if (values == null) return Util.Empty<SerializableStateElement>();
            var stateElementList = new List<SerializableStateElement>();
            var index = 0;
            foreach (var value in values)
            {
                var type = value.GetType();

                var isValueType = type.IsValueType || value is string;
                var isArray = value.GetType().IsArray || value is IEnumerable;
                if (value is string) isArray = false;

                if (isArray)
                {
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = $"{type.Name}[]",
                        TypeName = type.FullName,
                        Type = StateType.Array,
                        Children = GetChildren(value as IEnumerable, isProperty)
                    });
                }
                else if (!isValueType)
                {
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = "",
                        TypeName = type.FullName,
                        Type = StateType.Object,
                        Children = GetChildren(value, isProperty)
                    });
                }
                else
                {
                    stateElementList.Add(new SerializableStateElement
                    {
                        Name = $"{index}",
                        Value = $"{value}",
                        TypeName = type.FullName,
                        Type = StateType.Value,
                        Children = Util.Empty<SerializableStateElement>()
                    });
                }
                index++;
            }
            return stateElementList.ToArray();
        }

        private static void AddSerializableStateElement(string name, object value,
            Action<SerializableStateElement> listAddAction,
            bool isProperty)
        {
            var type = value.GetType();

            var isValueType = type.IsValueType || value is string;
            var isArray = value.GetType().IsArray || value is IEnumerable;
            if (value is string) isArray = false;

            var element = new SerializableStateElement
            {
                Name = name,
                Value = isArray ? $"{type.Name}[]" : isValueType ? $"{value}" : "",
                TypeName = type.FullName,
                Type = isArray ? StateType.Array : isValueType ? StateType.Value : StateType.Object,
                Children = Util.Empty<SerializableStateElement>()
            };

            if (isArray)
            {
                element.Children = GetChildren(value as IEnumerable, isProperty);
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
        public string Value;
        public SerializableStateElement[] Children;
        public string TypeName;
        public StateType Type;
    }

    public enum StateType
    {
        Value = 0,
        Array = 1,
        Object = 2
    }
#endif
}