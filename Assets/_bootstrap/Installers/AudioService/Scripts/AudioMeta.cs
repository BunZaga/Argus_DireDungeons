using UnityEngine;
using ZagaCore;

namespace FunZaga.Audio
{
    [CreateAssetMenu(fileName = nameof(AudioMeta), menuName = "Audio/" + nameof(AudioMeta))]
    public class AudioMeta : ScriptableObject
    {
        static private PseudoRandom rng = new PseudoRandom();

        [SerializeField] private AudioClip audioClip;
        public AudioClip AudioClip => audioClip;

        [SerializeField] private float volume = 1.0f;
        public float Volume => volume;

        [SerializeField] private TimingCurve frequencyVariance;
        public float FrequencyVariance => frequencyVariance?.Evaluate(rng.Next(0.0f, 1.0f)) ?? 0.0f;

        [SerializeField] private TimingCurve fadeIn;
        public TimingCurve FadeIn => fadeIn;

        [SerializeField] private TimingCurve fadeOut;
        public TimingCurve FadeOut => fadeOut;
    }
}