using System;
using UnityEngine;

namespace UniRedux
{
    public static class UniReduxMiddleware
    {
        /// <summary>
        /// Actionを非同期で実行するミドルウェア
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

        /// <summary>
        /// ログを表示する
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static Func<Dispatcher, Dispatcher> Logger<TState>(IStore<TState> store)
        {
            return next => action =>
            {
                Debug.Log($"<color=#8ced57>[Redux-Logger]</color> Dispatching for {action.GetType().Name}: {action}");
                var result = next(action);
                Debug.Log($"<color=#8ced57>[Redux-Logger]</color> Next state for {action.GetType().Name}: {store.GetState()}");
                return result;
            };
        }
    }
}