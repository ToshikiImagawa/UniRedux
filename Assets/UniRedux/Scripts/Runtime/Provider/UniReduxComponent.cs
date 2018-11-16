using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniRedux.Provider
{
    public interface IUniReduxComponent
    {
    }

    internal static class UniReduxComponentExtension
    {
        public static void InjectDispatcher(this IUniReduxComponent component)
        {
            var componentProperty = component?.GetType()
                .GetPrivateAndPublicProperty("Dispatch");
            if (
                componentProperty == null || componentProperty.PropertyType != typeof(Dispatcher) ||
                Attribute.GetCustomAttribute(componentProperty, typeof(UniReduxInjectAttribute)) == null
            ) return;
            componentProperty.GetSetMethod(true)?.Invoke(component, new object[]
            {
                (Dispatcher) UniReduxProvider.Store.Dispatch
            });
        }

        public static void SetProperty<TValue>(this IUniReduxComponent component, TValue value,
            MethodInfo componentMethodInfo, MethodInfo valueMethodInfo)
        {
            if (componentMethodInfo == null || valueMethodInfo == null || component == null) return;
            componentMethodInfo.Invoke(component, new[]
            {
                valueMethodInfo.Invoke(value, null)
            });
        }

        public static void InjectActionDispatcher<TActionDispatcher>(this IUniReduxComponent component,
            IEnumerable<PropertyInfo> componentProperties, TActionDispatcher actionDispatcher)
            where TActionDispatcher : class
        {
            var actionDispatcherProperties =
                typeof(TActionDispatcher).GetPublicProperties();
            var methodPairs = componentProperties.Join(actionDispatcherProperties, info => info.Name,
                    info => info.Name,
                    (componentProperty, actionDispatcherProperty)
                        => new Tuple<PropertyInfo, PropertyInfo>(
                            componentProperty, actionDispatcherProperty)
                )
                .Where(pair => pair.Item1.PropertyType == pair.Item2.PropertyType)
                .Select(pair => new Tuple<MethodInfo, MethodInfo>(
                    pair.Item1.GetSetMethod(true), pair.Item2.GetGetMethod()
                ))
                .Where(pair => pair.Item1 != null && pair.Item2 != null)
                .ToArray();
            foreach (var methodPair in methodPairs)
            {
                component.SetProperty(actionDispatcher, methodPair.Item1, methodPair.Item2);
            }
        }
    }
}