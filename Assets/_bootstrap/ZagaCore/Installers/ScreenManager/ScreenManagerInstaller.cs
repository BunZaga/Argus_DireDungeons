using RSG;
using UnityEngine;

namespace ZagaCore
{
    [CreateAssetMenu(fileName = nameof(ScreenManagerInstaller), menuName = "Services/" + nameof(ScreenManagerInstaller))]
    public class ScreenManagerInstaller : DepServiceInstaller
    {
        public override void Init()
        {
            Promise.All(Dependencies)
               .Then(() =>
               {
                   new ScreenManager();
                   Promise.Resolve();
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