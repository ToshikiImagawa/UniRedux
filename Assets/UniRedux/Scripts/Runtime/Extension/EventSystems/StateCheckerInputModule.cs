using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    public abstract class StateCheckerInputModule<TState> : BaseReduxInputModule<TState>
    {
        private bool _isCalledInactive;
        private readonly Dictionary<Type, IStateChecker> _checkers = new Dictionary<Type, IStateChecker>();
        private readonly Dictionary<Type, HashSet<Type>> _checkerChildren = new Dictionary<Type, HashSet<Type>>();
        private readonly HashSet<Type> _checkerRoots = new HashSet<Type>();

        private readonly Dictionary<Type, Action<GameObject, BaseEventData>> _executeEventsExecutors =
            new Dictionary<Type, Action<GameObject, BaseEventData>>();

        protected void Bind<TEventSystemHandler, TValue>(ExecuteEvents.EventFunction<TEventSystemHandler> eventHandler,
            Func<TState, TValue> selector, IEqualityComparer<TValue> equalityComparer = null,
            Func<BaseEventData, BaseEventData> middleware = null,
            Type parentEventSystemHandler = null)
            where TEventSystemHandler : IReduxEventSystemHandler
        {
            _checkers[typeof(TEventSystemHandler)] = new StateChecker<TValue>(selector, equalityComparer);
            _executeEventsExecutors[typeof(TEventSystemHandler)] = (target, data) =>
            {
                if (middleware != null) data = middleware(data);
                ExecuteEvents.Execute(target, data, eventHandler);
            };
            if (parentEventSystemHandler == null)
            {
                _checkerRoots.Add(typeof(TEventSystemHandler));
            }
            else
            {
                if (!_checkerChildren.ContainsKey(parentEventSystemHandler))
                {
                    _checkerChildren[parentEventSystemHandler] = new HashSet<Type>();
                }

                _checkerChildren[parentEventSystemHandler].Add(typeof(TEventSystemHandler));
            }
        }

        protected sealed override void Process(VoidMessage state)
        {
            if (!gameObject.activeSelf || !enabled)
            {
                _isCalledInactive = true;
                return;
            }

            Execute();
        }

        protected abstract void BindingPoint();

        private void Execute()
        {
            foreach (var checkerRoot in _checkerRoots)
            {
                ExecuteChecker(checkerRoot);
            }
        }

        private void ExecuteChecker(Type executeType)
        {
            if (!_checkers.ContainsKey(executeType)) Assert.CreateException();
            if (!_checkers[executeType].IsNewValue(CurrentStore.GetState())) return;
            var executeEventsExecutor = _executeEventsExecutors[executeType];
            EventSystem.Execute(executeType, executeEventsExecutor);
            if (!_checkerChildren.ContainsKey(executeType)) return;
            foreach (var checkerChild in _checkerChildren[executeType]) ExecuteChecker(checkerChild);
        }


        private class StateChecker<TValue> : IStateChecker
        {
            private readonly Func<TState, TValue> _selector;

            private TValue LastData { get; set; }

            private IEqualityComparer<TValue> EqualityComparer { get; }

            public bool IsNewValue(TState state)
            {
                var currentData = Select(state);
                if (EqualityComparer.Equals(currentData, LastData)) return false;
                LastData = currentData;
                return true;
            }

            private TValue Select(TState state)
            {
                return _selector == null ? default(TValue) : _selector.Invoke(state);
            }

            public StateChecker(Func<TState, TValue> selector, IEqualityComparer<TValue> equalityComparer = null)
            {
                _selector = selector;
                EqualityComparer = equalityComparer ?? EqualityComparer<TValue>.Default;
            }
        }

        private interface IStateChecker
        {
            bool IsNewValue(TState state);
        }

        protected virtual void Init()
        {
            StartMonitoring();
        }

        /// <summary>
        /// Before Execute OnEnable
        /// </summary>
        protected virtual void BeforeEnable()
        {
        }

        /// <summary>
        /// After Execute OnEnable
        /// </summary>
        protected virtual void AfterEnable()
        {
        }

        public void OnEnable()
        {
            BeforeEnable();
            if (_isCalledInactive) Execute();
            _isCalledInactive = false;
            AfterEnable();
        }

        public void Awake()
        {
            BindingPoint();
            Init();
        }
    }
}