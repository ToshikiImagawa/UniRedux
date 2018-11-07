using System;
using UnityEngine;

namespace UniRedux
{
    public static partial class UniReduxMiddleware
    {
        /// <summary>
        /// Display log
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