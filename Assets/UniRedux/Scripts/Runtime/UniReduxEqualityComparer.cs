using System;
using System.Collections.Generic;

namespace UniRedux
{
    public static class UniReduxEqualityComparer
    {
        private static readonly Dictionary<RuntimeTypeHandle, object> Cache =
            new Dictionary<RuntimeTypeHandle, object>();

        public static void SetEqualityComparer<T>(IEqualityComparer<T> equalityComparer)
        {
            Cache[typeof(T).TypeHandle] = equalityComparer;
        }

        public static IEqualityComparer<T> GetDefault<T>()
        {
            var t = typeof(T).TypeHandle;
            if (Cache.ContainsKey(t))
            {
                return (IEqualityComparer<T>) Cache[t];
            }

            return EqualityComparer<T>.Default;
        }
    }

    public static class UniReduxEqualityComparer<T>
    {
        public static IEqualityComparer<T> Default => UniReduxEqualityComparer.GetDefault<T>();

        public static void SetEqualityComparer(IEqualityComparer<T> equalityComparer)
        {
            UniReduxEqualityComparer.SetEqualityComparer(equalityComparer);
        }
    }
}