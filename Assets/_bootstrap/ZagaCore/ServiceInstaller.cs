using RSG;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZagaCore
{
    public interface IServiceInstaller
    {
        Promise Promise { get; }
        void Init();
    }

    [Serializable]
    public class ServiceInstaller : ScriptableObject, IServiceInstaller
    {
        public Promise Promise { get; } = new Promise();

        public virtual void Init()
        {
            throw new NotImplementedException(string.Format("ServiceInstaller:{0} Init() NOT IMPLEMENTED", name));
        }
    }

    [Serializable]
    public class DepServiceInstaller : ServiceInstaller
    {
        public List<Promise> Dependencies
        {
            get { return dependencies?.Select(dep => dep.Promise).ToList(); }
            protected set { }
        }

        [SerializeField]
        protected List<ServiceInstaller> dependencies;

        public override void Init()
        {
            Dependencies.Clear();
            Dependencies = null;
        }
    }
}