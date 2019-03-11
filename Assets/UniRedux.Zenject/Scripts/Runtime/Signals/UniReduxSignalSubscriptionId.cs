using System;
using System.Diagnostics;
using Zenject;

namespace UniRedux
{
    [DebuggerStepThrough]
    public struct UniReduxSignalSubscriptionId : IEquatable<UniReduxSignalSubscriptionId>
    {
        private UniReduxBindingId _signalId;
        private object _callback;

        public UniReduxBindingId SignalId
        {
            get { return _signalId; }
        }

        public object Callback
        {
            get { return _callback; }
        }

        public UniReduxSignalSubscriptionId(UniReduxBindingId signalId, object callback)
        {
            _signalId = signalId;
            _callback = callback;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _signalId.GetHashCode();
                hashCode = (hashCode * 397) ^ _callback.GetHashCode();
                return hashCode;
            }
        }

        public override bool Equals(object that)
        {
            if (that is UniReduxSignalSubscriptionId)
            {
                return Equals((UniReduxSignalSubscriptionId)that);
            }

            return false;
        }

        public bool Equals(UniReduxSignalSubscriptionId that)
        {
            return Equals(_signalId, that._signalId)
                && Equals(Callback, that.Callback);
        }

        public static bool operator ==(UniReduxSignalSubscriptionId left, UniReduxSignalSubscriptionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(UniReduxSignalSubscriptionId left, UniReduxSignalSubscriptionId right)
        {
            return !left.Equals(right);
        }
    }
}
