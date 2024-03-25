using System;
using UnityEngine;
using ZagaCore.Events.Input;

public class SpamTapProtectAbility : MonoBehaviour, ITouchUp
{
    [SerializeField] protected float tapDelay = 200/1000f;

    private float nextAvailableTime;
    
    public event Action<TouchEvent> onTouchUp;
    
    public void OnTouchUp(TouchEvent touchEvent)
    {
        if (Time.timeSinceLevelLoad < nextAvailableTime)
            return;
        
        nextAvailableTime = Time.timeSinceLevelLoad + tapDelay;
        onTouchUp?.Invoke(touchEvent);
    }
}
