using FunZaga.Audio;
using UnityEngine;
using ZagaCore;

namespace FunZaga.Events.Audio
{
    public class EventChangeAudioListener : GameEvent<EventChangeAudioListener>
    {
        public AudioListener AudioListener;

        public override void Recycle()
        {
            AudioListener = null;
        }
    }
    
    public class EventPlaySound : GameEvent<EventPlaySound>
    {
        public AudioDefinition AudioDefinition;

        public override void Recycle()
        {
            AudioDefinition = null;
        }
    }

    public class EventPlayMusic : GameEvent<EventPlayMusic>
    {
        public AudioDefinition AudioDefinition;
        public bool CrossFade = true;
        public float CrossFadeDuration = 1.0f;

        public override void Recycle()
        {
            AudioDefinition = null;
        }
    }
}