using System;
using System.Collections.Generic;

namespace UniRedux
{
    public class UniReduxStateDirector<TLocalState, TOriginalState> : IUniReduxStateDirector
    {
        private readonly IEqualityComparer<TLocalState> _equalityComparer;
        private readonly Func<TOriginalState, TLocalState> _converter;

        public UniReduxStateDirector(Func<TOriginalState, TLocalState> converter, IEqualityComparer<TLocalState> equalityComparer = null)
        {
            _converter = converter;
            _equalityComparer = equalityComparer ?? UniReduxEqualityComparer.GetDefault<TLocalState>();
        }

        public bool Checker(object leftState, object rightState)
        {
            if (leftState == null && rightState == null) return true;
            if (leftState == null || rightState == null) return false;
            if (!(leftState is TLocalState leftLocalState) || !(rightState is TLocalState rightLocalState))
                throw Assert.CreateException();
            return _equalityComparer.Equals(leftLocalState, rightLocalState);
        }

        public object Converter(object state)
        {
            if (!(state is TOriginalState originalState)) throw Assert.CreateException();
            return _converter.Invoke(originalState);
        }
    }

    public interface IUniReduxStateDirector
    {
        object Converter(object originalState);

        bool Checker(object leftState, object rightState);
    }
}
