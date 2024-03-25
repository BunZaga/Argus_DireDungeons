using UnityEngine;

namespace ZagaCore
{
    [CreateAssetMenu(fileName = nameof(PlatformUtilsInstaller), menuName = "Services/" + nameof(PlatformUtilsInstaller))]
    public class PlatformUtilsInstaller : ServiceInstaller
    {
        public override void Init()
        {
            new PlatformUtils();
            Promise.Resolve();
        }
    }
}