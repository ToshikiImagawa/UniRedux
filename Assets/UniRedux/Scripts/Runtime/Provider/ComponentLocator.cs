using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniRedux.Provider
{
    internal static class ComponentLocator
    {
        private static readonly Dictionary<RuntimeTypeHandle, ComponentInfo> ComponentInfoCash =
            new Dictionary<RuntimeTypeHandle, ComponentInfo>();

        public static void SetValue<TSource>(object component, TSource source)
        {
            if (component == null) throw Assert.CreateException("Component can not be null.");
            var key = component.GetType().TypeHandle;
            if (!ComponentInfoCash.ContainsKey(key)) ComponentInfoCash[key] = new ComponentInfo(component.GetType());
            ComponentInfoCash[key].SetValue(component, source);
        }

        private class ComponentInfo
        {
            private readonly Dictionary<PropertyKey, MethodInfo> _getSetMethodInfos;
            private readonly Dictionary<MethodKey, MethodInfo> _getMethodInfos;

            public ComponentInfo(Type type)
            {
                if (type == null) throw Assert.CreateException();
                _getSetMethodInfos = type.GetUniReduxInjectProperties().ToDictionary(
                    propertyInfo =>
                    {
                        var propertyName = propertyInfo.GetCustomAttribute<UniReduxInjectAttribute>()
                            ?.PropertyName;
                        if (string.IsNullOrEmpty(propertyName)) propertyName = propertyInfo.Name;
                        var key = new PropertyKey(propertyName, propertyInfo.PropertyType);
                        return key;
                    },
                    propertyInfo => propertyInfo.GetSetMethod(true));
                _getMethodInfos = type.GetUniReduxInjectMethods()
                    .ToDictionary(methodInfo =>
                    {
                        return new MethodKey(methodInfo.Name, methodInfo.GetParameters().Select(parameterInfo =>
                        {
                            var propertyName = parameterInfo.GetCustomAttribute<UniReduxInjectAttribute>()
                                ?.PropertyName;
                            if (string.IsNullOrEmpty(propertyName)) propertyName = parameterInfo.Name;
                            var key = new PropertyKey(propertyName, parameterInfo.ParameterType);
                            return key;
                        }).ToArray());
                    }, methodInfo => methodInfo);
            }

            public void SetValue<TSource>(object component, TSource source)
            {
                var valueCache = new Dictionary<PropertyKey, object>();

                foreach (var setMethodInfo in _getSetMethodInfos)
                {
                    if (!valueCache.ContainsKey(setMethodInfo.Key))
                    {
                        object val;
                        if (!LocalStateLocator.TryGetValue(setMethodInfo.Key, source, out val)) continue;
                        valueCache[setMethodInfo.Key] = val;
                    }

                    setMethodInfo.Value.Invoke(component, new[] {valueCache[setMethodInfo.Key]});
                }

                foreach (var methodInfo in _getMethodInfos)
                {
                    var isContinue = false;
                    foreach (var propertyKey in methodInfo.Key.Parameters)
                    {
                        if (valueCache.ContainsKey(propertyKey)) continue;
                        object val;
                        if (!LocalStateLocator.TryGetValue(propertyKey, source, out val))
                        {
                            isContinue = true;
                            break;
                        }

                        valueCache[propertyKey] = val;
                    }

                    if (isContinue) continue;

                    methodInfo.Value.Invoke(component,
                        methodInfo.Key.Parameters.Select(parameter => valueCache[parameter]).ToArray());
                }
            }
        }

        private struct MethodKey : IEquatable<MethodKey>
        {
            private string MethodName { get; }
            public PropertyKey[] Parameters { get; }

            public MethodKey(string methodName, PropertyKey[] parameters)
            {
                Parameters = parameters;
                MethodName = methodName;
            }

            public bool Equals(MethodKey other)
            {
                return string.Equals(MethodName, other.MethodName) && Parameters.SequenceEqual(other.Parameters);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is MethodKey && Equals((MethodKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((MethodName != null ? MethodName.GetHashCode() : 0) * 397) ^
                           (Parameters != null ? Parameters.GetHashCode() : 0);
                }
            }
        }
    }
}