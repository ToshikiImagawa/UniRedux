using System;

namespace UniRedux
{
    public static partial class UniReduxMiddleware
    {
        /// <summary>
        /// Executes Action asynchronously
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static Func<Dispatcher, Dispatcher> Thunk<TState>(IStore<TState> store)
        {
            return next => action =>
            {
                var thunkAction = action as ThunkAction<TState>;
                if (thunkAction == null) return next(action);
                thunkAction.Action(store.Dispatch, store.GetState);
                return thunkAction;
            };
        }
    }
}