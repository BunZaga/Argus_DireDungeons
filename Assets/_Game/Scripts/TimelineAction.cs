using System;
using FunZaga.Audio;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using ZagaCore;

public class TimelineAction : MonoBehaviour
{
    public float Priority => priority;
    [SerializeField] private float priority;
    
    public PlayableDirector playableDirector;
    public ActionControl ActionControl => actionControl;
    private ActionControl actionControl;

    public Transform SourceTransform => sourceTransform;
    private Transform sourceTransform;

    public void SetActionData(ActionInitData initData)
    {
        actionControl = initData.ActionControl;
        sourceTransform = initData.Source.transform;
        SetSource(initData.Source);
    }
    private void SetSource(GameObject sourceGO)
    {
        var timelineAsset = (TimelineAsset)playableDirector.playableAsset;
        foreach (var output in timelineAsset.outputs)
        {
            var track = output.sourceObject;
            if (track == null)
                continue;
            switch (track.name)
            {
                case "Animation Track":
                    playableDirector.SetGenericBinding(track, sourceGO);
                    break;
                case "Action & Movement":
                    playableDirector.SetGenericBinding(track, this);
                    break;
                case "Recycle":
                    playableDirector.SetGenericBinding(track, this);
                    break;
                case "Play Audio":
                    playableDirector.SetGenericBinding(track, GetComponent<AudioController3d>());
                    break;
                case "Weapon Trail":
                    playableDirector.SetGenericBinding(track, sourceGO.GetComponentInChildren<WeaponRoot>(true));
                    break;
            }
            
        }
    }
    
    public void PlayTimeline()
    {
        gameObject.SetActive(true);
    }
    
    public void InterruptTimeline()
    {
        playableDirector.Stop();
    }
}