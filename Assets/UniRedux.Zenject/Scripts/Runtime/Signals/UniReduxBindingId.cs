using System;
using System.Diagnostics;

namespace UniRedux
{
    [DebuggerStepThrough]
    public struct UniReduxBindingId : IEquatable<UniReduxBindingId>
    {
        private Type _localStateType;
        private object _identifier;

        public Type LocalStateType
        {
            get { return _localStateType; }
            set { _localStateType = value; }
        }

        public object Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        public static UniReduxBindingId Empty { get; } = new UniReduxBindingId();

        public UniReduxBindingId(Type localStateType, object identifier)
        {
            _localStateType = localStateType;
            _identifier = identifier;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UniReduxBindingId)obj);
        }
        public bool Equals(UniReduxBindingId other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (other._localStateType != _localStateType) return false;
            if (other._identifier != _identifier) return false;
            return true;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _localStateType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (_identifier != null ? _identifier.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UniReduxBindingId left, UniReduxBindingId right)
        {
            return left.LocalStateType == right.LocalStateType && Equals(left.Identifier, right.Identifier);
        }

        public static bool operator !=(UniReduxBindingId left, UniReduxBindingId right)
        {
            return !left.Equals(right);
        }
    }
}
