using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        /// <summary>
        /// Immutable update state checker
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="store"></param>
        /// <returns></returns>
        public static Func<Dispatcher, Dispatcher> CheckImmutableUpdate<TState>(IStore<TState> store)
        {
            return next => action =>
            {
                var state = store.GetState();
                byte[] beforState;
                byte[] afterState;
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, state);
                    beforState = ms.ToArray();
                }
                var result = next(action);
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, state);
                    afterState = ms.ToArray();
                }
                if (beforState.Length != afterState.Length)
                {
                    Debug.LogError($"<color=#8ced57>[Redux-CheckImmutableUpdate]</color> Not an immutable update for {action.GetType().Name}: {action}");
                    return result;
                }
                for (int i = 0; i < beforState.Length; i++)
                {
                    if (beforState[i] != afterState[i])
                    {
                        Debug.LogError($"<color=#8ced57>[Redux-CheckImmutableUpdate]</color> Not an immutable update for {action.GetType().Name}: {action}");
                        return result;
                    }
                }
                return result;
            };

        }

        private static bool CompareArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }
            return true;
        }
    }
}