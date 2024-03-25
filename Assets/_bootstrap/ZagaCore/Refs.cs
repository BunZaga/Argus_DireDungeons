using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZagaCore
{

    [Serializable]
    public class Refs
    {
        private static Refs instance { get; set; } = new Refs();

        public static Refs Instance => instance;

        private Dictionary<Type, object> binder = new Dictionary<Type, object>();
        private Dictionary<Type, List<object>> waitForList = new Dictionary<Type, List<object>>();

        public void Bind<T>(T requested) where T : class
        {
            var interfaceType = typeof(T);
            if (interfaceType.IsInterface)
            {
                if (binder.ContainsKey(interfaceType))
                {
                    Debug.LogErrorFormat("binder already contains service: {0}", interfaceType);
                    return;
                }
                binder[interfaceType] = requested;
                
                if (waitForList.ContainsKey(interfaceType))
                {
                    for (int i = 0, ilen = waitForList[interfaceType].Count; i < ilen; i++)
                    {
                        ((Promise<T>)waitForList[interfaceType][i]).Resolve((T)requested);
                    }
                    waitForList[interfaceType].Clear();
                    waitForList.Remove(interfaceType);
                }
            }
            else
            {
                var requestedType = requested.GetType();
                if (binder.ContainsKey(requestedType))
                {
                    Debug.LogErrorFormat("binder already contains service: {0}", requestedType);
                    return;
                }
                binder[requestedType] = requested;

                if (waitForList.ContainsKey(requestedType))
                {
                    for (int i = 0, ilen = waitForList[requestedType].Count; i < ilen; i++)
                    {
                        ((Promise<T>)waitForList[requestedType][i]).Resolve(requested);
                    }
                    waitForList[requestedType].Clear();
                    waitForList.Remove(requestedType);
                }
            }
        }
        
        public void Refresh<T>(T requested) where T : class
        {
            var interfaceType = typeof(T);
            if (interfaceType.IsInterface)
            {
                if (!binder.ContainsKey(interfaceType))
                {
                    Bind(requested);
                    return;
                }
                binder[interfaceType] = requested;
            }
            else
            {
                var requestedType = requested.GetType();
                if (!binder.ContainsKey(requestedType))
                {
                    Bind(requestedType);
                    return;
                }
                binder[requestedType] = requested;
            }
        }
        
        /*public void BindInterface<T>(object requested) where T : class
        {
            var interfaceType = typeof(T);
            if (binder.ContainsKey(interfaceType))
            {
                Debug.LogErrorFormat("binder already contains service: {0}", interfaceType);
                return;
            }
            binder[interfaceType] = requested;

            if (waitForList.ContainsKey(interfaceType))
            {
                for (int i = 0, ilen = waitForList[interfaceType].Count; i < ilen; i++)
                {
                    ((Promise<T>)waitForList[interfaceType][i]).Resolve((T)requested);
                }
                waitForList[interfaceType].Clear();
                waitForList.Remove(interfaceType);
            }
        }*/
        
        public void Unbind<T>(T requested) where T: class
        {
            var interfaceType = requested.GetType();
            
            if (interfaceType.IsInterface)
            {
                if (binder.TryGetValue(interfaceType, out _))
                {
                    binder.Remove(interfaceType);
                }
            }
            else
            {
                Unbind<T>();
            }
        }

        public void Unbind<T>() where T: class
        {
            var interfaceType = typeof(T);
            
            if (binder.TryGetValue(interfaceType, out _))
            {
                binder.Remove(interfaceType);
            }
        }
        
        public bool Contains<T>(T requested)
        {
            var requestedType = typeof(T);
            return binder.TryGetValue(requestedType, out var returnService);
        }

        public bool Contains<T>()
        {
            var requestedType = typeof(T);
            return binder.TryGetValue(requestedType, out var returnService);
        }
        
        public T Get<T>() where T : class
        {
            var requestedType = typeof(T);
            binder.TryGetValue(requestedType, out var returnService);
            return (T) returnService;
        }

        public IPromise<T> WaitFor<T>()
        {
            var requestedType = typeof(T);
            object returnService = null;
            binder.TryGetValue(requestedType, out returnService);

            var waitFor = new Promise<T>();

            if (returnService != null)
            {
                waitFor.Resolve((T)returnService);
            }
            else
            {
                if (!waitForList.ContainsKey(requestedType))
                {
                    waitForList.Add(requestedType, new List<object>());
                }
                waitForList[requestedType].Add(waitFor);
            }
            return waitFor;
        }
    }
}