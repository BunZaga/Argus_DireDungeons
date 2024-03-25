using System;
using System.Collections.Generic;

namespace ZagaCore
{
    public class EventService
    {
        private Dictionary<Type, IInvokerLinkedList> invokerMap = new Dictionary<Type, IInvokerLinkedList>();
        private Dictionary<Type, Queue<IGameEvent>> eventPool = new Dictionary<Type, Queue<IGameEvent>>();

        public EventService()
        {
            Refs.Instance.Bind(this);
        }
        
        public T GetPooledEvent<T>() where T: class, IGameEvent, new()
        {
            var requestedType = typeof(T);
            if(eventPool.TryGetValue(requestedType, out var eventPoolQueue))
            {
                if(eventPoolQueue.Count > 0)
                {
                    var gameEvent = eventPoolQueue.Dequeue() as T;
                    return gameEvent;
                }
            }
            else
            {
                eventPool.Add(requestedType, new Queue<IGameEvent>());
            }
            return new T() as T;
        }

        public void ClearEventPool<T>() where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (eventPool.ContainsKey(requestedType))
            {
                eventPool[requestedType].Clear();
                eventPool.Remove(requestedType);
            }
        }

        public void Subscribe<T>(Action handler, uint priority = 0) where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (!invokerMap.ContainsKey(requestedType))
            {
                invokerMap.Add(requestedType, new InvokerLinkedList());
            }
            var invoker = invokerMap[requestedType];
            invoker.TryAdd(handler, priority);
        }

        public void Subscribe<T>(Action<T> handler, uint priority = 0) where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (!invokerMap.ContainsKey(requestedType))
            {
                invokerMap.Add(requestedType, new InvokerLinkedList<T>());
            }
            var invoker = invokerMap[requestedType];
            invoker.TryAdd(handler, priority);
        }

        public void UnSubscribe<T>(Action handler) where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (!invokerMap.ContainsKey(requestedType))
            {
                return;
            }
            var invoker = invokerMap[requestedType];
            invoker.TryRemove(handler);
        }

        public void UnSubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (!invokerMap.ContainsKey(requestedType))
            {
                return;
            }
            var invoker = invokerMap[requestedType];
            invoker.TryRemove(handler);
        }

        public void Dispatch<T>(GameEvent<T> gameEvent, bool autoRecycle = true) where T: class, IGameEvent
        {
            var requestedType = typeof(T);

            if (!invokerMap.ContainsKey(requestedType))
            {
                return;
            }
            
            var invoker = invokerMap[requestedType];
            invoker.Invoke(gameEvent);

            if (!autoRecycle || !eventPool.ContainsKey(requestedType)) return;
            gameEvent.Recycle();
            eventPool[requestedType].Enqueue(gameEvent);
        }

        public void Dispatch<T>() where T : class, IGameEvent
        {
            var requestedType = typeof(T);
            if (!invokerMap.ContainsKey(requestedType))
            {
                return;
            }
            
            var invoker = invokerMap[requestedType];
            invoker?.Invoke();
        }

        public void OnNextDispatch<T>(Action action) where T : GameEvent
        {
            void Callback()
            {
                action.Invoke();
                UnSubscribe<T>(Callback);
            }
            Subscribe<T>(Callback);
        }

        public void OnNextDispatch<T>(Action<T> action) where T : GameEvent<T>
        {
            void Callback(T gameEvent)
            {
                action.Invoke(gameEvent);
                UnSubscribe<T>(Callback);
            }
            Subscribe<T>(Callback);
        }

        public void Clear<T>() where T : IGameEvent
        {
            var requestedType = typeof(T);
            if (invokerMap.ContainsKey(requestedType))
            {
                invokerMap[requestedType].Clear();
            }
        }

        public void Recycle<T>(GameEvent<T> gameEvent) where T: class, IGameEvent
        {
            var requestedType = typeof(T);
            if (!eventPool.ContainsKey(requestedType)) return;

            gameEvent.Recycle();
            eventPool[requestedType].Enqueue(gameEvent);
        }
    }
}