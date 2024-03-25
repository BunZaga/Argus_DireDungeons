using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Loading;

namespace FunZaga.Audio
{
    public class AudioController2d : MonoBehaviour
    {
        [SerializeField] private AudioChannel audioChannel;
        public AudioChannel AudioChannel => audioChannel;

        [SerializeField] private int maxAudioSources;

        private struct AudioMapping
        {
            public AudioDefinition AudioDefinition;
            public AudioMeta AudioMeta;
            public AudioSource AudioSource => audioSource;
            private AudioSource audioSource;

            public AudioMapping(AudioSource audioSource, AudioDefinition audioDefinition, AudioMeta audioMeta)
            {
                this.audioSource = audioSource;
                this.AudioDefinition = audioDefinition;
                this.AudioMeta = audioMeta;
            }
        }

        private Dictionary<AudioDefinition, int> activeAudioCount = new Dictionary<AudioDefinition, int>();
        private Dictionary<AudioSource, AudioMapping> audioSourceActive = new Dictionary<AudioSource, AudioMapping>();
        private Queue<AudioMapping> audioSourcePool = new Queue<AudioMapping>();
        private List<AudioMapping> audioDone = new List<AudioMapping>();

        private Coroutine activeAudioLoop = null;

        private void Awake()
        {
            for (int i = 0; i < maxAudioSources; i++)
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioChannel.AudioMixerGroup;

                audioSourcePool.Enqueue(new AudioMapping(audioSource, null, null));
            }

            if (Refs.Instance.Contains<AudioService>())
                OnAllServicesLoaded();
            else
            {
                var eventService = Refs.Instance.Get<EventService>();
                eventService.OnNextDispatch<AllServicesLoaded>(OnAllServicesLoaded);
            }
        }

        private void OnAllServicesLoaded()
        {
            var audioService = Refs.Instance.Get<AudioService>();
            audioService.Register2dController(this);
        }

        public void PlayAudioDefinition(AudioDefinition audioDefinition)
        {
            var audioMeta = audioDefinition.PeekAudioMeta();
            var maxSources = maxAudioSources;

            int count = 0;
            activeAudioCount.TryGetValue(audioDefinition, out count);

            if (count < maxSources)
            {
                if (audioSourcePool.Count > 0)
                {
                    audioMeta = audioDefinition.PopAudioMeta();
                    var audioMapping = audioSourcePool.Dequeue();
                    var audioSource = audioMapping.AudioSource;
                    audioMapping.AudioDefinition = audioDefinition;
                    audioMapping.AudioMeta = audioMeta;

                    audioSourceActive.Add(audioSource, audioMapping);

                    activeAudioCount[audioDefinition] = count + 1;

                    audioSource.clip = audioMeta.AudioClip;
                    audioSource.volume = audioMeta.Volume;
                    audioSource.pitch = 1.0f + audioMeta.FrequencyVariance;
                    audioSource.loop = audioDefinition.Loop;
                    if (audioMeta.FadeIn != null)
                    {
                        audioSource.volume = 0.0f;
                    }
                    audioSource.Play();

                    if (activeAudioLoop == null)
                    {
                        activeAudioLoop = StartCoroutine(ActiveAudioLoop());
                    }
                }
            }
        }

        private IEnumerator ActiveAudioLoop()
        {
            while (audioSourceActive.Count > 0)
            {
                using (var e = audioSourceActive.GetEnumerator())
                {
                    while (e.MoveNext())
                    {
                        var audioSource = e.Current.Key;
                        var audioMapping = e.Current.Value;
                        if (audioSource.isPlaying)
                        {
                            var audioMeta = audioMapping.AudioMeta;
                            if (audioMeta.FadeIn != null && audioSource.volume < audioMeta.Volume)
                            {
                                audioSource.volume = Mathf.Min(audioMeta.FadeIn.Evaluate(audioSource.time), audioMeta.Volume);
                            }
                            if (audioMeta.FadeOut != null && audioSource.volume > 0)
                            {
                                var fadeDuration = audioMeta.FadeOut.Duration;
                                var startFade = audioSource.clip.length - fadeDuration;
                                if (audioSource.time > startFade)
                                {
                                    audioSource.volume = Mathf.Max(audioMeta.FadeOut.Evaluate(audioSource.time - startFade), 0);
                                }
                            }
                            continue;
                        }
                        audioDone.Add(audioMapping);
                    }
                }

                if (audioDone.Count > 0)
                {
                    for (int i = 0, iMax = audioDone.Count; i < iMax; i++)
                    {
                        var audioMap = audioDone[i];
                        var audioSource = audioMap.AudioSource;
                        audioSource.Stop();
                        audioSource.clip = null;
                        var audioDefinition = audioMap.AudioDefinition;
                        var count = activeAudioCount[audioDefinition];
                        activeAudioCount[audioDefinition] = Mathf.Max(count - 1, 0);
                        audioSourceActive.Remove(audioSource);
                        audioMap.AudioDefinition = null;
                        audioMap.AudioMeta = null;
                        audioSourcePool.Enqueue(audioMap);
                    }
                    audioDone.Clear();
                }
                yield return null;
            }

            activeAudioLoop = null;
        }
    }
}
