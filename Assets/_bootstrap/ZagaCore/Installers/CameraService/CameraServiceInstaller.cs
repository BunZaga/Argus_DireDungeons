using UnityEngine;

namespace ZagaCore
{
    [CreateAssetMenu(fileName = nameof(CameraServiceInstaller), menuName = "Services/" + nameof(CameraServiceInstaller))]
    public class CameraServiceInstaller : ServiceInstaller
    {
        public override void Init()
        {
            new CameraService();
            Promise.Resolve();
        }
    }
}