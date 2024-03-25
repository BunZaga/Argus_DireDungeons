using UnityEngine;
using ZagaCore;
using RSG;
using FunZaga;

[CreateAssetMenu(fileName = nameof(InputServiceInstaller), menuName = "Services/" + nameof(InputServiceInstaller))]
public class InputServiceInstaller : DepServiceInstaller
{
    public override void Init()
    {
        Promise.All(Dependencies)
           .Then(() =>
           {
               var appRoot = Refs.Instance.Get<AppRoot>();
               appRoot.ServiceRoot.AddComponent<InputService>();
               Refs.Instance.WaitFor<InputService>().Then((_)=> {
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
