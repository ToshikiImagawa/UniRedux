using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace UniRedux
{
    public static partial class UniReduxMiddleware
    {
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
                byte[] befogState;
                byte[] afterState;
                var bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, state);
                    befogState = ms.ToArray();
                }

                var result = next(action);
                var newState = store.GetState();
                
                if(!state.Equals(newState)) return result;

                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, state);
                    afterState = ms.ToArray();
                }

                if (befogState.Length != afterState.Length)
                {
                    Debug.LogError(
                        $"<color=#8ced57>[Redux-CheckImmutableUpdate]</color> Not an immutable update for {action.GetType().Name}: {action}");
                    return result;
                }

                if (!befogState.Where((t, i) => t != afterState[i]).Any()) return result;
                Debug.LogError(
                    $"<color=#8ced57>[Redux-CheckImmutableUpdate]</color> Not an immutable update for {action.GetType().Name}: {action}");
                return result;
            };
        }
    }
}