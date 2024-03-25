using RSG;
using UnityEngine;
using ZagaCore;

namespace FunZaga
{
    [CreateAssetMenu(fileName = nameof(MainThreadDispatcherInstaller), menuName = "Services/" + nameof(MainThreadDispatcherInstaller))]
    public class MainThreadDispatcherInstaller : DepServiceInstaller
    {
        public override void Init()
        {
            Promise.All(Dependencies)
               .Then(() =>
               {
                   var appRoot = Refs.Instance.Get<AppRoot>();
                   appRoot.gameObject.AddComponent<MainThreadDispatcher>();
                   Refs.Instance.WaitFor<MainThreadDispatcher>().Then((_)=> {
                       Promise.Resolve();
                   }).Catch((err) =>
                   {
                       Debug.Log(err.Message);
                       Debug.Log(err.StackTrace);
                   });
               })
                .Catch((err) =>
                {
                    Debug.Log(err.Message);
                    Debug.Log(err.StackTrace);
                });

            base.Init();
        }
    }
}