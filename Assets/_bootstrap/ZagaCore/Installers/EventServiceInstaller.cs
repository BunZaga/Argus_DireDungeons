using UnityEngine;

namespace ZagaCore
{
    [CreateAssetMenu(fileName = nameof(EventServiceInstaller), menuName = "Services/" + nameof(EventServiceInstaller))]
    public class EventServiceInstaller : ServiceInstaller
    {
        public override void Init()
        {
            new EventService();
            Promise.Resolve();
        }
    }
}