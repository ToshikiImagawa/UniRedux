using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniRedux.Examples
{
    public static class CacheComponent
    {
        private static readonly IDictionary<string, Component> CacheComponents = new Dictionary<string, Component>();

        public static T GetComponentFindNameInChildren<T>(this Component component, string name) where T : Component
        {
            return component.gameObject.GetComponentFindNameInChildren<T>(name);
        }

        private static T GetComponentFindNameInChildren<T>(this GameObject gameObject, string name) where T : Component
        {
            var key = $"{gameObject.GetInstanceID()}__{name}__{typeof(T).Name}";
            if (CacheComponents.ContainsKey(key)) return CacheComponents[key] as T;

            var components = gameObject.GetComponentsInChildren<T>(false);
            var hitComponent = components.FirstOrDefault(component => component.gameObject.name == name);
            if (hitComponent != null) CacheComponents.Add(key, hitComponent);
            return hitComponent;
        }
    }
}