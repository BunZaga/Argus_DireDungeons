using UnityEngine;
using UnityEngine.Audio;

namespace FunZaga.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioChannel), menuName = "Audio/" + nameof(AudioChannel))]
    public class AudioChannel : ScriptableObject
    {
        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;
        [SerializeField] private AudioMixerGroup audioMixerGroup;

        private const string SFX_PREFIX = "SFX_";
        private string key;

        private void OnEnable()
        {
            key = SFX_PREFIX + name;
        }

        public bool IsOn
        {
            get
            {
                return PlayerPrefsX.GetBool(key, true);
            }
            set
            {
                if (value != PlayerPrefsX.GetBool(key))
                {
                    PlayerPrefsX.SetBool(key, value);
                    // TODO: Raise event?
                }
            }
        }
    }
}
