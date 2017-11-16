using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UniRedux.Sample
{
    public static class Util
    {
        public static T[] Empty<T>()
        {
            return (T[]) Enumerable.Empty<T>();
        }

        /// <summary>
        /// Create disposer
        /// </summary>
        /// <param name="disposeListener"></param>
        /// <returns></returns>
        public static IDisposable CreateDisposer(Action disposeListener)
        {
            return new Disposer(disposeListener);
        }

        private class Disposer : IDisposable
        {
            private Action _disposeListener;

            public Disposer(Action disposeListener)
            {
                _disposeListener = disposeListener;
            }

            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        _disposeListener?.Invoke();
                    }

                    disposedValue = true;
                }
            }

            void IDisposable.Dispose()
            {
                Dispose(true);
            }
        }
    }

    public static class CacheComponent
    {
        private static IDictionary<string, Component> _cacheComponents = new Dictionary<string, Component>();

        public static T GetComponentFindNameInChildren<T>(this Component component, string name) where T : Component
        {
            return component.gameObject.GetComponentFindNameInChildren<T>(name);
        }

        public static T GetComponentFindNameInChildren<T>(this GameObject gameObject, string name) where T : Component
        {
            var key = $"{gameObject.GetInstanceID()}__{name}__{typeof(T).Name}";
            if (_cacheComponents.ContainsKey(key)) return _cacheComponents[key] as T;

            var components = gameObject.GetComponentsInChildren<T>(false);
            var hitComponent = components.FirstOrDefault(component => component.gameObject.name == name);
            if (hitComponent != null) _cacheComponents.Add(key, hitComponent);
            return hitComponent;
        }
    }
}