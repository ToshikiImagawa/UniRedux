using System;
using System.Diagnostics;

namespace UniRedux
{
    [DebuggerStepThrough]
    public struct UniReduxBindingId : IEquatable<UniReduxBindingId>
    {
        private Type _localStateType;
        private Type _originalStateType;
        private object _identifier;

        public Type LocalStateType
        {
            get { return _localStateType; }
            set { _localStateType = value; }
        }
        public Type OriginalStateType
        {
            get { return _originalStateType; }
            set { _originalStateType = value; }
        }

        public object Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        public UniReduxBindingId(Type localStateType, Type originalStateType, object identifier)
        {
            _localStateType = localStateType;
            _originalStateType = originalStateType;
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
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(null, other._localStateType)) return false;
            if (ReferenceEquals(null, other._originalStateType)) return false;
            return _localStateType == other._localStateType &&
                _originalStateType == other._originalStateType &&
                _identifier.Equals(other._identifier);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _localStateType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ _originalStateType?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (_identifier != null ? _identifier.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(UniReduxBindingId left, UniReduxBindingId right)
        {
            return left.LocalStateType == right.LocalStateType && left.OriginalStateType == right.OriginalStateType && Equals(left.Identifier, right.Identifier);
        }

        public static bool operator !=(UniReduxBindingId left, UniReduxBindingId right)
        {
            return !left.Equals(right);
        }
    }
}
