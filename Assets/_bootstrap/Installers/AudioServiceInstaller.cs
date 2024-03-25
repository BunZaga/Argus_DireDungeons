using UnityEngine;
using ZagaCore;

namespace FunZaga.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioServiceInstaller), menuName = "Services/" + nameof(AudioServiceInstaller))]
    public class AudioServiceInstaller : ServiceInstaller
    {
        public override void Init()
        {
            new AudioService();
            Promise.Resolve();
        }
    }
}