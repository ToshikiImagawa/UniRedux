using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UniRedux.Editor
{
    public static class UniDebug
    {
        public static SerializableStateElement ToSerialize(this object state)
        {
            return StateReflection.Serialize(state);
        }
    }

    internal static class StateReflection
    {
        private static readonly int MaxNestedCount = 20;
        private const BindingFlags AllMemberBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        public static SerializableStateElement Serialize(object state)
        {
            return new SerializableStateElement
            {
                Name = $"{state.GetType().Name}",
                Value = "",
                Type = state.GetType(),
                ObjectType = ObjectType.Object,
                Children = GetChildren(state)
            };
        }

        private static SerializableStateElement[] GetChildren(object obj, int nestCount = 0)
        {
            if (obj == null) return Array.Empty<SerializableStateElement>();
            if (nestCount > MaxNestedCount) return Array.Empty<SerializableStateElement>();

            var stateElementList = new List<SerializableStateElement>();

            try
            {
                var targetType = obj.GetType();

                var targetProperties = targetType.GetPublicPropertyInfos();
                foreach (var propertyInfo in targetProperties)
                {
                    var name = propertyInfo.Name;
                    var value = propertyInfo.GetValue(obj);
                    var type = propertyInfo.PropertyType;

                    AddSerializableStateElement(name, value, type, stateElementList.Add, nestCount);
                }

                var targetFields = targetType.GetPublicFieldInfos();
                foreach (var targetField in targetFields)
                {
                    var name = targetField.Name;
                    var value = targetField.GetValue(obj);
                    var type = targetField.FieldType;

                    AddSerializableStateElement(name, value, type, stateElementList.Add, nestCount);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return stateElementList.ToArray();
        }

        private static SerializableStateElement[] GetChildrenEnumerable(IEnumerable values,
            int nestCount = 0)
        {
            if (values == null) return Enumerable.Empty<SerializableStateElement>().ToArray();
            var valuesType = values.GetType();
            var stateElementList = new List<SerializableStateElement>();
            var arrayValues = values as object[] ?? values.Cast<object>().ToArray();
            if (valuesType.IsGenericType)
            {
                var dictionary = values as IDictionary;
                if (dictionary != null)
                {
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        var value = entry.Value;
                        AddSerializableStateElement($"{entry.Key}", value, value.GetType(), stateElementList.Add,
                            nestCount);
                    }
                }
                else
                {
                    for (var index = 0; index < arrayValues.Length; index++)
                    {
                        var value = arrayValues[index];
                        AddSerializableStateElement($"{index}", value, value.GetType(), stateElementList.Add,
                            nestCount);
                    }
                }
            }
            else if (valuesType.IsArray)
            {
                for (var index = 0; index < arrayValues.Length; index++)
                {
                    var value = arrayValues[index];
                    AddSerializableStateElement($"{index}", value, value.GetType(), stateElementList.Add, nestCount);
                }
            }

            return stateElementList.ToArray();
        }

        private static void AddSerializableStateElement(string name, object value, Type type,
            Action<SerializableStateElement> listAddAction, int nestCount = 0)
        {
            var element = new SerializableStateElement
            {
                Name = name,
                Value = string.Empty,
                Type = type,
                ObjectType = ObjectType.Object,
                Children = Enumerable.Empty<SerializableStateElement>().ToArray()
            };

            if (value == null)
            {
                element.Value = "Null";
                listAddAction(element);
                return;
            }

            if (IsNotChildren(type, value))
            {
                element.ObjectType = ObjectType.Value;
                element.Value = value;
                listAddAction(element);
                return;
            }

            var enumerable = value as IEnumerable;

            if (enumerable != null)
            {
                element.Value = $"Not support　type {type.Name}";

                if (type.IsGenericType)
                {
                    var genericArguments = type.GetGenericArguments();
                    var dictionary = enumerable as IDictionary;
                    var list = enumerable as IList;
                    var collection = enumerable as ICollection;
                    if (dictionary != null)
                    {
                        if (genericArguments.Length >= 2)
                        {
                            element.Value = $"IDictionary<{genericArguments[0].Name},{genericArguments[1].Name}>";
                        }
                    }
                    else if (list != null)
                    {
                        if (genericArguments.Length >= 1)
                        {
                            element.Value = $"IList<{genericArguments[0].Name}>";
                        }
                    }
                    else if (collection != null)
                    {
                        if (genericArguments.Length >= 1)
                        {
                            element.Value = $"ICollection<{genericArguments[0].Name}>";
                        }
                    }
                    else
                    {
                        if (genericArguments.Length >= 1)
                        {
                            element.Value = $"IEnumerable<{genericArguments[0].Name}>";
                        }
                    }
                }
                else if (type.IsArray)
                {
                    element.Value = $"{type?.GetElementType()?.Name}[]";
                }

                element.ObjectType = ObjectType.Array;
                element.Children = GetChildrenEnumerable(enumerable, nestCount + 1);
            }
            else
            {
                element.ObjectType = ObjectType.Object;
                element.Children = GetChildren(value, nestCount + 1);
            }

            listAddAction(element);
        }

        private static IEnumerable<PropertyInfo> GetPublicPropertyInfos(this Type self)
        {
            return self?.GetProperties(AllMemberBindingFlags) ?? Array.Empty<PropertyInfo>();
        }

        private static IEnumerable<FieldInfo> GetPublicFieldInfos(this Type self)
        {
            return self?.GetFields(AllMemberBindingFlags) ?? Array.Empty<FieldInfo>();
        }

        private static bool IsNotChildren(Type type, object value)
        {
            return type.IsPrimitive ||
                   type.IsEnum ||
                   value is string ||
                   value is DateTime ||
                   value is TimeSpan ||
                   value is decimal ||
                   value is Type;
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
}