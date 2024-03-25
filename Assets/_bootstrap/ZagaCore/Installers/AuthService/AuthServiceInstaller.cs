using RSG;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Plugins.Auth
{
    [CreateAssetMenu(fileName = nameof(AuthServiceInstaller), menuName = "Services/" + nameof(AuthServiceInstaller))]
    public class AuthServiceInstaller : DepServiceInstaller
    {
        public override void Init()
        {
            Debug.Log("---------------->AuthServiceInstaller started.");
            Promise.All(Dependencies)
               .Then(() =>
               {
                   Debug.Log("---------------->AuthServiceInstaller dependencies done.");
                   new AuthService();

                   Refs.Instance.WaitFor<AuthService>().Then((_) => {
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