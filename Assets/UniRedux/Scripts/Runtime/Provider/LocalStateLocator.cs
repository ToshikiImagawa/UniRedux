using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UniRedux.Provider
{
    internal static class LocalStateLocator
    {
        public static PropertyKey[] GetPropertyKeys<TSource>()
        {
            return LocalStateInfo<TSource>.PropertyKeys;
        }

        public static object GetValue<TSource>(PropertyKey propertyKey, TSource source)
        {
            return LocalStateInfo<TSource>.GetValue(propertyKey, source);
        }

        public static bool TryGetValue<TSource>(PropertyKey propertyKey, TSource source, out object result)
        {
            return LocalStateInfo<TSource>.TryGetValue(propertyKey, source, out result);
        }

        private static class LocalStateInfo<TSource>
        {
            private static readonly Dictionary<PropertyKey, MethodInfo> GetGetMethodInfos;

            public static PropertyKey[] PropertyKeys => GetGetMethodInfos.Keys.ToArray();

            static LocalStateInfo()
            {
                var type = typeof(TSource);
                if (type == null) throw Assert.CreateException();
                GetGetMethodInfos = type.GetPublicProperties().ToDictionary(
                    propertyInfo => new PropertyKey(propertyInfo.Name, propertyInfo.PropertyType),
                    propertyInfo => propertyInfo.GetGetMethod());
            }

            public static object GetValue(PropertyKey propertyKey, TSource source)
            {
                if (!GetGetMethodInfos.ContainsKey(propertyKey))
                    throw Assert.CreateException($" {propertyKey.PropertyName}");
                return source == null
                    ? propertyKey.PropertyType.GetDefault()
                    : GetGetMethodInfos[propertyKey].Invoke(source, null);
            }

            public static bool TryGetValue(PropertyKey propertyKey, TSource source, out object result)
            {
                if (!GetGetMethodInfos.ContainsKey(propertyKey))
                {
                    result = propertyKey.PropertyType.GetDefault();
                    return false;
                }

                result = source == null
                    ? propertyKey.PropertyType.GetDefault()
                    : GetGetMethodInfos[propertyKey].Invoke(source, null);
                return true;
            }
        }
    }
}