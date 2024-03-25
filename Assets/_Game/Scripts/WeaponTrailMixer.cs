using UnityEngine;
using UnityEngine.Playables;

public class WeaponTrailMixer : PlayableBehaviour
{
    private bool wasActive = false;
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var weaponRoot = playerData as WeaponRoot;
        if (weaponRoot == null)
            return;
        
        bool isPlaying = false;
        Material currentMat = null;
        
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; ++i)
        {
            var inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0.0f)
            {
                ScriptPlayable<WeaponTrailDefBehaviour> inputPlayable =
                    (ScriptPlayable<WeaponTrailDefBehaviour>)playable.GetInput(i);

                var input = inputPlayable.GetBehaviour();
                currentMat = input.TrailMaterial;
                isPlaying = true;
            }
        }

        if (wasActive != isPlaying)
        {
            weaponRoot.SetTrailActive(isPlaying);
            weaponRoot.SetMaterial(currentMat);
            wasActive = isPlaying;
        }
    }
}
