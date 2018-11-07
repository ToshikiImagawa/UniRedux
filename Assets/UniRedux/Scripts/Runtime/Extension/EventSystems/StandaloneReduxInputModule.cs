using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRedux.EventSystems
{
    public abstract class StandaloneReduxInputModule<TState> : BaseReduxInputModule<TState>
    {
        private readonly IDictionary<Type, IEventPair> _eventPairsHash = new Dictionary<Type, IEventPair>();
        private readonly IList<Type> _eventRootList = new List<Type>();
        private readonly IDictionary<Type, IList<Type>> _eventChildrenHash = new Dictionary<Type, IList<Type>>();

        public void AddExecuteEvents<T>(ExecuteEvents.EventFunction<T> eventHandler, Func<bool> filter,
            Func<BaseEventData> eventCreator = null, Type parentType = null) where T : IReduxEventSystemHandler
        {
            var eventPair = new EventPair<T>(eventHandler, filter, parentType, eventCreator);
            _eventPairsHash.Add(eventPair.Key, eventPair);

            if (eventPair.ParentType == null)
            {
                if (_eventRootList.Contains(eventPair.Key)) return;
                _eventRootList.Add(eventPair.Key);
            }
            else
            {
                if (!_eventChildrenHash.ContainsKey(eventPair.ParentType))
                {
                    _eventChildrenHash.Add(eventPair.ParentType, new List<Type>());
                }

                if (_eventChildrenHash[eventPair.ParentType].Contains(eventPair.Key)) return;
                _eventChildrenHash[eventPair.ParentType].Add(eventPair.Key);
            }
        }

        protected override void Process(VoidMessage voidMessage)
        {
            if (_eventRootList.Count == 0) return;
            foreach (var rootType in _eventRootList)
            {
                if (!_eventPairsHash.ContainsKey(rootType)) continue;
                Run(_eventPairsHash[rootType]);
            }
        }

        private void Run(IEventPair eventPair)
        {
            if (!eventPair.Filter()) return;
            if (EventSystem.TargetObjectsCount(eventPair.Key) > 0)
            {
                EventSystem.Execute(eventPair.Key, eventPair.Execute, eventPair.Data);
            }

            if (!_eventChildrenHash.ContainsKey(eventPair.Key)) return;
            foreach (var childrenType in _eventChildrenHash[eventPair.Key])
            {
                if (_eventPairsHash.ContainsKey(childrenType)) Run(_eventPairsHash[childrenType]);
            }
        }
    }

    internal class EventPair<T> : IEventPair where T : IReduxEventSystemHandler
    {
        private readonly Action<GameObject, BaseEventData> _executeEventsAction;
        private readonly Func<bool> _filter;
        private readonly Func<BaseEventData> _eventCreator;

        public EventPair(ExecuteEvents.EventFunction<T> eventHandler, Func<bool> filter, Type parentType,
            Func<BaseEventData> eventCreator)
        {
            _executeEventsAction += (target, data) => { ExecuteEvents.Execute(target, data, eventHandler); };
            _filter += filter;
            ParentType = parentType;
            _eventCreator = eventCreator;
        }

        void IEventPair.Execute(GameObject target, BaseEventData data)
        {
            _executeEventsAction?.Invoke(target, data);
        }

        bool IEventPair.Filter()
        {
            return _filter?.Invoke() ?? false;
        }

        public Type Key => typeof(T);
        public Type ParentType { get; }
        public BaseEventData Data => _eventCreator?.Invoke();
    }

    internal interface IEventPair
    {
        void Execute(GameObject target, BaseEventData data = null);

        bool Filter();

        Type Key { get; }

        Type ParentType { get; }
        BaseEventData Data { get; }
    }
}