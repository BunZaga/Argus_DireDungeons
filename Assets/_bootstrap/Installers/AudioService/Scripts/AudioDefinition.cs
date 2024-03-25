using UnityEngine;
using ZagaCore;

namespace FunZaga.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioDefinition), menuName = "Audio/" + nameof(AudioDefinition))]
    public class AudioDefinition : ScriptableObject
    {
        static private PseudoRandom rng = new PseudoRandom();

        private enum AudioPoolType
        {
            Single,
            Sequential,
            Random
        }

        [SerializeField] private AudioPoolType audioPoolType;

        [SerializeField] private AudioChannel audioChannel;
        public AudioChannel AudioChannel => audioChannel;
        
        [SerializeField] private bool loop;
        public bool Loop => loop;

        [SerializeField] private AudioMeta[] audioPool;

        private int index = 0;

        public AudioMeta PeekAudioMeta()
        {
            return audioPool[index];
        }

        public AudioMeta PopAudioMeta()
        {
            var audioMeta = PeekAudioMeta();
            AdvanceToNextClip();
            return audioMeta;
        }

        private void AdvanceToNextClip()
        {
            if (audioPool.Length == 0)
            {
                Debug.LogWarningFormat("AudioDefinition [{0}] no AudioClips", name);
                return;
            }

            switch (audioPoolType)
            {
                case AudioPoolType.Single:
                    index = 0;
                    break;
                case AudioPoolType.Sequential:
                    index = (index + 1) % audioPool.Length;
                    break;
                case AudioPoolType.Random:
                    index = rng.Next(audioPool.Length);
                    break;
            }
        }

        public void OnEnable()
        {
            index = -1;
            if(audioPool.Length == 1)
            {
                audioPoolType = AudioPoolType.Single;
            }
            AdvanceToNextClip();
        }
    }
}