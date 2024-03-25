using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZagaCore;
using ZagaCore.Events.Loading;
using Random = System.Random;

namespace FunZaga.Audio
{
    public class AudioController3d : MonoBehaviour
    {
        [SerializeField] private AudioChannel audioChannel;
        public AudioChannel AudioChannel => audioChannel;

        [SerializeField] private float maxDistance;
        public float MaxDistance => maxDistance;

        [SerializeField] private int maxAudioSources;

        [SerializeField] private AudioDefinition audioDefinition;

        private struct AudioMapping
        {
            public AudioDefinition AudioDefinition;
            public AudioMeta AudioMeta;
            public AudioSource AudioSource => audioSource;
            private AudioSource audioSource;
            public Transform Transform;

            public AudioMapping(AudioSource audioSource, AudioDefinition audioDefinition, AudioMeta audioMeta, Transform transform)
            {
                this.audioSource = audioSource;
                this.AudioDefinition = audioDefinition;
                this.AudioMeta = audioMeta;
                this.Transform = transform;
            }
        }

        private Dictionary<AudioDefinition, int> activeAudioCount = new Dictionary<AudioDefinition, int>();
        private Dictionary<AudioSource, AudioMapping> audioSourceActive = new Dictionary<AudioSource, AudioMapping>();
        private Queue<AudioMapping> audioSourcePool = new Queue<AudioMapping>();
        private List<AudioMapping> audioDone = new List<AudioMapping>();

        private Coroutine activeAudioLoop = null;

        public bool IsPlaying => activeAudioLoop != null;

        private void Awake()
        {
            for (int i = 0; i < maxAudioSources; i++)
            {
                var go = new GameObject(string.Format("{0}[{1}]", "AudioSource", i));
                var audioSource = go.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = audioChannel.AudioMixerGroup;
                audioSource.maxDistance = maxDistance;
                audioSource.spatialBlend = 1.0f;
                audioSource.spread = 130f;
                audioSource.rolloffMode = AudioRolloffMode.Custom;

                var audioMapping = new AudioMapping(audioSource, null, null, go.transform);
                audioMapping.Transform.position = go.transform.position;
                audioMapping.Transform.gameObject.SetActive(false);
                audioSourcePool.Enqueue(audioMapping);
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
            Debug.Log("AudioController:OnAllServicesLoaded");
            var audioService = Refs.Instance.Get<AudioService>();
            audioService.Register3dController(this);
        }

        public void SetAudioDefinition(AudioDefinition audioDef)
        {
            audioDefinition = audioDef;
        }
        
        public void Stop()
        {
            // TODO: later make this be able to take a single transform
            foreach (var kvp in audioSourceActive)
            {
                audioDone.Add(kvp.Value);
            }

            if (audioDone.Count > 0)
            {
                for (int i = 0, iMax = audioDone.Count; i < iMax; i++)
                {
                    var audioMap = audioDone[i];
                    var audioSource = audioMap.AudioSource;
                    audioSourcePool.Enqueue(audioMap);
                    audioSource.Stop();
                    audioSource.clip = null;
                    var audioDefinition = audioMap.AudioDefinition;
                    var count = activeAudioCount[audioDefinition];
                    activeAudioCount[audioDefinition] = Mathf.Max(count - 1, 0);
                    audioSourceActive.Remove(audioSource);
                    audioMap.AudioDefinition = null;
                    audioMap.AudioMeta = null;
                }
                audioDone.Clear();
            }

            if (audioSourceActive.Count == 0)
            {
                if (activeAudioLoop != null)
                {
                    StopCoroutine(activeAudioLoop);
                    activeAudioLoop = null;
                }
            }
        }

        private void OnDisable()
        {
            foreach (var kvp in audioSourceActive)
            {
                audioDone.Add(kvp.Value);
            }
            
            if (audioDone.Count > 0)
            {
                for (int i = 0, iMax = audioDone.Count; i < iMax; i++)
                {
                    var audioMap = audioDone[i];
                    var audioSource = audioMap.AudioSource;
                    audioSourcePool.Enqueue(audioMap);
                    audioSource.Stop();
                    audioSource.clip = null;
                    var audioDefinition = audioMap.AudioDefinition;
                    var count = activeAudioCount[audioDefinition];
                    activeAudioCount[audioDefinition] = Mathf.Max(count - 1, 0);
                    audioSourceActive.Remove(audioSource);
                    audioMap.AudioDefinition = null;
                    audioMap.AudioMeta = null;
                }
                audioDone.Clear();
            }

            if (audioSourceActive.Count == 0)
            {
                if (activeAudioLoop != null)
                {
                    StopCoroutine(activeAudioLoop);
                    activeAudioLoop = null;
                }
            }
        }

        public void PlayAudioDefinition(Transform target)
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

                    audioMapping.Transform.position = target.position;
                    audioMapping.Transform.gameObject.SetActive(true);

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
                                audioSource.volume = Mathf.Min(audioMeta.FadeIn.Evaluate(audioSource.time),
                                    audioMeta.Volume);
                            }

                            if (audioMeta.FadeOut != null && audioSource.volume > 0)
                            {
                                var fadeDuration = audioMeta.FadeOut.Duration;
                                var startFade = audioSource.clip.length - fadeDuration;
                                if (audioSource.time > startFade)
                                {
                                    audioSource.volume =
                                        Mathf.Max(audioMeta.FadeOut.Evaluate(audioSource.time - startFade), 0);
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
                        audioSourcePool.Enqueue(audioMap);
                        audioSource.Stop();
                        audioSource.clip = null;
                        var audioDefinition = audioMap.AudioDefinition;
                        var count = activeAudioCount[audioDefinition];
                        activeAudioCount[audioDefinition] = Mathf.Max(count - 1, 0);
                        audioSourceActive.Remove(audioSource);
                        audioMap.AudioDefinition = null;
                        audioMap.AudioMeta = null;
                        audioMap.Transform.gameObject.SetActive(false);
                    }

                    audioDone.Clear();
                }

                yield return null;
            }

            if (activeAudioLoop != null)
            {
                StopCoroutine(activeAudioLoop);
                activeAudioLoop = null;
            }
        }
    }
}