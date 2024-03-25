using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(WeaponRoot))]
[TrackClipType(typeof(WeaponTrailDefClip))]
public class WeaponTrailDefTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<WeaponTrailMixer>.Create(graph, inputCount);
    }
}
