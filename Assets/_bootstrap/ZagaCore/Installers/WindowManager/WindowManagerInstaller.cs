using System.Collections.Generic;
using UnityEngine;
using ZagaCore;
using RSG;

namespace FunZaga.WindowService
{
    [CreateAssetMenu(fileName = nameof(WindowManagerInstaller), menuName = "Services/" + nameof(WindowManagerInstaller))]
    public class WindowManagerInstaller : DepServiceInstaller
    {
        [SerializeField] private List<WindowConfig> windowConfigs;

        public override void Init()
        {
            Promise.All(Dependencies)
               .Then(() =>
               {
                   new WindowManager(windowConfigs);
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
