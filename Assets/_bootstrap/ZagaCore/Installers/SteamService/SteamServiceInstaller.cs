using UnityEngine;
using ZagaCore;
using RSG;

#if UNITY_STEAM
using FunZaga.Events.Steam;
#endif

namespace FunZaga.Plugins.Steam
{
    [CreateAssetMenu(fileName = nameof(SteamServiceInstaller), menuName = "Services/" + nameof(SteamServiceInstaller))]
    public class SteamServiceInstaller : DepServiceInstaller
    {
        [SerializeField] private GameObject steamManagerPrefab;

        public override void Init()
        {
#if UNITY_STEAM
            Debug.Log("---------------->SteamServiceInstaller started.");
            Promise.All(Dependencies)
               .Then(() =>
               {

                   var appRoot = Refs.Instance.Get<AppRoot>();
                   var steamManagerGO = Instantiate(steamManagerPrefab, appRoot.ServiceRoot.transform, false);
                   var steamManager = steamManagerGO.GetComponent<SteamManager>();
                   Refs.Instance.Bind(steamManager);
                   steamManagerGO.AddComponent<SteamService>();
                   
                   Refs.Instance.WaitFor<SteamService>().Then((_) =>
                   {
                       Debug.Log("---------------->SteamServiceInstaller completed.");
                       Refs.Instance.Get<EventService>().Dispatch<SteamLoginSuccess>();
                       Promise.Resolve();
                   });
               })
                .Catch((err) =>
                {
                    Debug.Log(err.Message);
                    Debug.Log(err.StackTrace);
                });
#else
            Promise.Resolve();
#endif
            
            base.Init();
        }
    }
}