using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZagaCore
{
    public interface IInvokerNode
    {
        uint Id { get; }
        IInvokerNode Next { get; set; }
        IInvokerNode Prev { get; set; }

        int Priority { get; set; }
        object Handler { get; set; }
    }

    public class InvokerNode : IInvokerNode
    {
        private static uint nodeId = 0;
        private static HashSet<IInvokerNode> nodePool = new HashSet<IInvokerNode>();

        public uint Id { get; }
        public IInvokerNode Next { get; set; }
        public IInvokerNode Prev { get; set; }

        public int Priority { get; set; }
        public object Handler { get; set; }
        public Action Action => Handler as Action;

        private InvokerNode()
        {
            Id = nodeId++;
        }

        public static IInvokerNode Create(Action action)
        {
            IInvokerNode node;
            if(nodePool.Count > 0)
            {
                node = nodePool.First();
                nodePool.Remove(node);
                node.Handler = action;
            }
            else
            {
                node = new InvokerNode
                {
                    Handler = action
                };
            }
            return node;
        }

        public static void Recycle(IInvokerNode node)
        {
            node.Next = null;
            node.Prev = null;
            node.Priority = 0;
            node.Handler = null;

            if (nodePool.Contains(node))
            {
                return;
            }

            nodePool.Add(node);
        }
    }

    public class InvokerNode<T> : IInvokerNode where T : IGameEvent
    {
        private static uint nodeId = 0;
        private static HashSet<IInvokerNode> nodePool = new HashSet<IInvokerNode>();

        public uint Id { get; }
        public IInvokerNode Next { get; set; }
        public IInvokerNode Prev { get; set; }

        public int Priority { get; set; }
        public object Handler { get; set; }
        public Action<T> Action => Handler as Action<T>;

        private InvokerNode()
        {
            Id = nodeId++;
        }

        public static IInvokerNode Create(Action<T> action)
        {
            IInvokerNode node;
            if (nodePool.Count > 0)
            {
                node = nodePool.First();
                nodePool.Remove(node);
                node.Handler = action;
            }
            else
            {
                node = new InvokerNode<T>
                {
                    Handler = action
                };
            }
            return node;
        }

        public static void Recycle(IInvokerNode node)
        {
            node.Next = null;
            node.Prev = null;
            node.Priority = 0;
            node.Handler = null;

            if (nodePool.Contains(node))
            {
                return;
            }
            nodePool.Add(node);
        }
    }
}