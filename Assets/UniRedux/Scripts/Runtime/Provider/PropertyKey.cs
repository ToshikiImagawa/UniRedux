using System;

namespace UniRedux.Provider
{
    internal struct PropertyKey : IEquatable<PropertyKey>
    {
        public string PropertyName { get; }
        public Type PropertyType { get; }

        public PropertyKey(string name, Type type)
        {
            PropertyName = name;
            PropertyType = type;
        }

        public bool Equals(PropertyKey other)
        {
            return string.Equals(PropertyName, other.PropertyName) &&
                   PropertyType.TypeHandle.Equals(other.PropertyType.TypeHandle);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PropertyKey && Equals((PropertyKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PropertyName != null ? PropertyName.GetHashCode() : 0) * 397) ^
                       PropertyType.TypeHandle.GetHashCode();
            }
        }
    }
}