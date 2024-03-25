using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using ZagaCore;
using FunZaga.Events.Audio;

namespace FunZaga.Audio
{
    public class AudioService
    {
        private EventService eventService =>_eventService ?? (_eventService = Refs.Instance.Get<EventService>());
        private EventService _eventService;

        private Dictionary<AudioChannel, AudioController2d> audioControllers = new Dictionary<AudioChannel, AudioController2d>();
        private Dictionary<AudioController3d, AudioController3d> audioControllers3d = new Dictionary<AudioController3d, AudioController3d>();

        private HashSet<AudioListener> audioListeners = new HashSet<AudioListener>();
        
        public AudioService()
        {
            eventService.Subscribe<EventPlaySound>(OnEventPlaySound);
            eventService.Subscribe<EventPlayMusic>(OnEventPlayMusic);
            eventService.Subscribe<EventChangeAudioListener>(OnEventListenerChanged);
            Refs.Instance.Bind(this);
        }

        private void OnEventListenerChanged(EventChangeAudioListener evnt)
        {
            if (!audioListeners.Contains(evnt.AudioListener))
            {
                audioListeners.Add(evnt.AudioListener);
            }

            using (var e = audioListeners.GetEnumerator())
            {
                while (e.MoveNext())
                {
                    var listener = e.Current;
                    listener.enabled = listener == evnt.AudioListener;
                }
            }
        }
        
        private void OnEventPlaySound(EventPlaySound eventPlaySound)
        {
            Debug.Log("Playing sound from event:" + eventPlaySound.AudioDefinition.name);
            PlaySound(eventPlaySound.AudioDefinition);
        }

        private void OnEventPlayMusic(EventPlayMusic eventPlayMusic)
        {
        }

        public void Register2dController(AudioController2d audioController)
        {
            if (audioControllers.ContainsKey(audioController.AudioChannel))
            {
                Debug.LogWarning("Audio channel already registered...");
                return;
            }
            audioControllers[audioController.AudioChannel] = audioController;
        }

        public void Register3dController(AudioController3d audioController3d)
        {
            if (audioControllers3d.ContainsKey(audioController3d))
            {
                Debug.LogWarning("Audio channel already registered...");
                return;
            }
            audioControllers3d[audioController3d] = audioController3d;
        }

        public void PlaySound(AudioDefinition audioDefinition)
        {
            AudioController2d audioController;
            Debug.Log("Playing sound " + audioDefinition.PeekAudioMeta().name);
            if (audioControllers.TryGetValue(audioDefinition.AudioChannel, out audioController))
            {
                if (audioController.AudioChannel.IsOn)
                {
                    audioController.PlayAudioDefinition(audioDefinition);
                }
            }
        }

        public void Stop(AudioController3d audioController3d, Transform transform)
        {
            if (audioControllers3d.TryGetValue(audioController3d, out audioController3d))
            {
                if (audioController3d.AudioChannel.IsOn)
                {
                    audioController3d.Stop();
                }
            }
        }

        public void PlaySound(AudioController3d audioController3d, Transform transform)
        {
            if (audioControllers3d.TryGetValue(audioController3d, out audioController3d))
            {
                if (audioController3d.AudioChannel.IsOn)
                {
                    audioController3d.PlayAudioDefinition(transform);
                }
            }
        }
    }
}
