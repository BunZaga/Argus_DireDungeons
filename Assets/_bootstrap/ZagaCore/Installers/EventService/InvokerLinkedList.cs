using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZagaCore
{
    public interface IInvokerLinkedList
    {
        bool IsEmpty { get; }
        void Invoke(IGameEvent gameEvent = null);
        bool Contains(object obj);
        void TryAdd(object obj, uint priority = 0);
        void TryRemove(object obj);
        void Clear();
    }

    public class InvokerLinkedList : IInvokerLinkedList
    {
        private IInvokerNode first;
        private IInvokerNode last;

        private Dictionary<object, IInvokerNode> activeNodes = new Dictionary<object, IInvokerNode>();

        public void Invoke(IGameEvent gameEvent)
        {
            var node = first;

            while (node != null)
            {
                var next = node.Next;
                ((InvokerNode)node).Action?.Invoke();
                node = next;
            }
        }

        public bool IsEmpty => first == null;

        public bool Contains(object obj)
        {
            return activeNodes.ContainsKey(obj);
        }

        public void TryAdd(object obj, uint priority) 
        {
            if (!(obj is Action))
            {
                // is there a better way to enforce this???
                Debug.LogError("Trying to add Action<T> to Action List:"+obj);
                return;
            }
            if (activeNodes.ContainsKey(obj))
            {
                Debug.LogWarningFormat("listener {0} already added!", obj);
                return;
            }

            TryRemove(obj);
            IInvokerNode node;

            if (first == null)
            {
                node = first = last = InvokerNode.Create(obj as Action);
                activeNodes.Add(obj, node);
                return;
            }

            if (priority < first.Priority)
            {
                node = first.Prev = InvokerNode.Create(obj as Action);
                node.Next = first;
                first = node;
                activeNodes.Add(obj, node);
                return;
            }
            if (priority >= last.Priority)
            {
                node = last.Next = InvokerNode.Create(obj as Action);
                node.Prev = last;
                last = node;
                activeNodes.Add(obj, node);
                return;
            }

            var before = first.Next;
            while (before != null && before.Priority <= priority)
            {
                before = before.Next;
            }

            node = InvokerNode.Create(obj as Action);

            if (before.Prev != null)
            {
                before.Prev.Next = node;
            }
            before.Prev = node;
            activeNodes.Add(obj, node);
        }

        public void TryRemove(object obj)
        {

            if (!activeNodes.TryGetValue(obj, out IInvokerNode node))
                return;

            activeNodes.Remove(obj);

            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            if (node.Equals(first))
            {
                first = node.Next;
            }
            if (node.Equals(last))
            {
                last = node.Prev;
            }
            InvokerNode.Recycle(node);
            return;
        }

        public void Clear()
        {
            var node = first;

            while (node != null)
            {
                var next = node.Next;
                if (activeNodes.ContainsKey(node.Handler))
                {
                    activeNodes.Remove(node.Handler);
                }
                InvokerNode.Recycle(node);
                node = node.Next;
            }
        }
    }

    public class InvokerLinkedList<T> : IInvokerLinkedList where T : IGameEvent
    {
        private IInvokerNode first;
        private IInvokerNode last;

        private Dictionary<object, IInvokerNode> activeNodes = new Dictionary<object, IInvokerNode>();

        public void Invoke(IGameEvent gameEvent)
        {
            var node = first;

            if (node == null)
                return;
            
            while (node != null)
            {
                var next = node.Next;
                ((InvokerNode<T>)node).Action.Invoke((T)gameEvent);
                node = next;
            }
        }

        public bool IsEmpty => first == null;

        public bool Contains(object obj)
        {
            return activeNodes.ContainsKey(obj);
        }

        public void TryAdd(object obj, uint priority)
        {
            if (!(obj is Action<T>))
            {
                // is there a better way to enforce this???
                Debug.LogError("Trying to add Action to Action<T> List:"+obj);
                return;
            }
            if (activeNodes.ContainsKey(obj))
            {
                Debug.LogWarningFormat("listener {0} already added!", obj);
                return;
            }

            TryRemove(obj);
            IInvokerNode node;

            if (first == null)
            {
                node = first = last = InvokerNode<T>.Create(obj as Action<T>);
                activeNodes.Add(obj, node);
                return;
            }

            if (priority < first.Priority)
            {
                node = InvokerNode<T>.Create(obj as Action<T>);
                first.Prev = node;
                node.Next = first;
                first = node;
                activeNodes.Add(obj, node);
                return;
            }
            if (priority >= last.Priority)
            {
                node = InvokerNode<T>.Create(obj as Action<T>);
                last.Next = node;
                node.Prev = last;
                last = node;
                activeNodes.Add(obj, node);
                return;
            }

            var before = first.Next;
            while (before != null && before.Priority <= priority)
            {
                before = before.Next;
            }

            node = InvokerNode<T>.Create(obj as Action<T>);

            if (before.Prev != null)
            {
                before.Prev.Next = node;
            }
            before.Prev = node;
            activeNodes.Add(obj, node);
        }

        public void TryRemove(object obj)
        {

            if (!activeNodes.TryGetValue(obj, out IInvokerNode node))
                return;

            activeNodes.Remove(obj);
            if (node.Prev != null)
            {
                node.Prev.Next = node.Next;
            }
            if (node.Next != null)
            {
                node.Next.Prev = node.Prev;
            }
            if (node.Equals(first))
            {
                first = node.Next;
            }
            if (node.Equals(last))
            {
                last = node.Prev;
            }
            InvokerNode<T>.Recycle(node);
        }

        public void Clear()
        {
            var node = first;

            while (node != null)
            {
                var next = node.Next;
                if (activeNodes.ContainsKey(node.Handler))
                {
                    activeNodes.Remove(node.Handler);
                }
                InvokerNode<T>.Recycle(node);
                node = node.Next;
            }
        }
    }
}