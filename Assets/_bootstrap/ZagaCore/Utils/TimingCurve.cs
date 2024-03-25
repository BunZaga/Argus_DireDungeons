using UnityEngine;

namespace ZagaCore
{
    [CreateAssetMenu(fileName = nameof(TimingCurve), menuName = "Timing/" + nameof(TimingCurve))]
    public class TimingCurve : ScriptableObject
    {
        [SerializeField] private AnimationCurve animationCurve;

        public float Duration => animationCurve.keys[animationCurve.length - 1].time;

        public float Evaluate(float atTime)
        {
            return animationCurve.Evaluate(atTime);
        }


    }
}
