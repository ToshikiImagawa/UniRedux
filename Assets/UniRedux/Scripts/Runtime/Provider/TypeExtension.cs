using System;
using System.Linq;
using System.Reflection;

namespace UniRedux.Provider
{
    internal static class TypeExtension
    {
        private const BindingFlags PrivateAndPublicBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.FlattenHierarchy;

        private const BindingFlags PublicBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static |
            BindingFlags.Public |
            BindingFlags.FlattenHierarchy;

        public static PropertyInfo[] GetPrivateAndPublicProperties(this Type self)
        {
            return self?.GetProperties(PrivateAndPublicBindingFlags) ?? Array.Empty<PropertyInfo>();
        }

        public static MethodInfo[] GetPrivateAndPublicMethods(this Type self)
        {
            return self?.GetMethods(PrivateAndPublicBindingFlags) ?? Array.Empty<MethodInfo>();
        }

        public static PropertyInfo[] GetPublicProperties(this Type self)
        {
            return self?.GetProperties(PublicBindingFlags) ?? Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo GetPrivateAndPublicProperty(this Type self, string name)
        {
            return self?.GetProperty(name, PrivateAndPublicBindingFlags);
        }

        public static PropertyInfo GetPublicProperty(this Type self, string name)
        {
            return self?.GetProperty(name, PublicBindingFlags);
        }

        public static PropertyInfo[] GetUniReduxInjectProperties(this Type self)
        {
            return self.GetPrivateAndPublicProperties()
                .Where(property =>
                    Attribute.GetCustomAttribute(
                        property, typeof(UniReduxInjectAttribute)
                    ) != null
                ).ToArray();
        }

        public static MethodInfo[] GetUniReduxInjectMethods(this Type self)
        {
            return self?.GetPrivateAndPublicMethods()
                .Where(method =>
                    Attribute.GetCustomAttribute(
                        method, typeof(UniReduxInjectAttribute)
                    ) != null
                ).ToArray();
        }

        public static object GetDefault(this Type self)
        {
            return self.IsValueType ? Activator.CreateInstance(self) : null;
        }
    }
}