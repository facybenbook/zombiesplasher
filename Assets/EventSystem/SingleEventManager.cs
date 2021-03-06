﻿// Copyright (C) 2016 Filip Cyrus Bober

using System.Collections.Generic;

namespace FCB.EventSystem
{
    public class SingleEventManager
    {
        private class EventKey
        {
            public readonly int Id;

            private readonly System.Type _eventType;

            public EventKey(int id, System.Type eventType)
            {
                Id = id;
                _eventType = eventType;
            }

            protected bool Equals(EventKey other)
            {
                return Id == other.Id && _eventType == other._eventType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((EventKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked { return (Id * 397) ^ _eventType.GetHashCode(); }
            }
        }

        public static SingleEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SingleEventManager();
                }

                return _instance;
            }
        }

        private static SingleEventManager _instance = null;

        public delegate void EventDelegate<T>(T e) where T : GameEvent, ISingleEvent;
        private delegate void EventDelegate(ISingleEvent e);

        /// <summary>
        /// Key is SingleEvent hash, generated by unique GameObjectId and event type
        /// </summary>
        private Dictionary<EventKey, EventDelegate> delegates = new Dictionary<EventKey, EventDelegate>();

        public void AddListener<T>(int ownerId, EventDelegate<T> eventListener) where T : GameEvent, ISingleEvent
        {
            var key = new EventKey(ownerId, typeof(T));

            if (delegates.ContainsKey(key))
                return;

            EventDelegate internalDelegate = (e) => eventListener((T)e);
            EventDelegate eventInvoker;
            if (delegates.TryGetValue(key, out eventInvoker))
            {
                eventInvoker += internalDelegate;
                delegates[key] = eventInvoker;
            }
            else
            {
                delegates[key] = internalDelegate;
            }
        }

        public void RemoveListener<T>(int ownerId, EventDelegate<T> eventListener) where T : GameEvent, ISingleEvent
        {
            var key = new EventKey(ownerId, typeof(T));

            EventDelegate internalDelegate;
            if (delegates.TryGetValue(key, out internalDelegate))
            {
                EventDelegate eventInvoker;
                if (delegates.TryGetValue(key, out eventInvoker))
                {
                    eventInvoker -= internalDelegate;
                    if (eventInvoker == null)
                    {
                        delegates.Remove(key);
                    }
                    else
                    {
                        delegates[key] = eventInvoker;
                    }
                }
            }
        }

        public void Raise<T>(int ownerId, T e) where T : GameEvent, ISingleEvent
        {
            var key = new EventKey(ownerId, e.GetType());

            EventDelegate eventInvoker;
            if (delegates.TryGetValue(key, out eventInvoker))
            {
                eventInvoker.Invoke(e);
            }
        }

    }
}
