using System;

namespace UniRedux
{
    /// <summary>
    /// 非同期で実行するActionを登録するAction
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public class ThunkAction<TState>
    {
        public Action<Dispatcher, Func<TState>> Action { get; }

        public ThunkAction(Action<Dispatcher, Func<TState>> action)
        {
            Action = action;
        }
    }
}
