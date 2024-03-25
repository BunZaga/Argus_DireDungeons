using UnityEngine;
using UnityEngine.Playables;

public class WeaponTrailDefClip : PlayableAsset
{
    public Material TrailMaterial;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<WeaponTrailDefBehaviour>.Create(graph);
        var weaponTrail = playable.GetBehaviour();
        weaponTrail.TrailMaterial = TrailMaterial;
        return playable;
    }
}
