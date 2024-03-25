using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class ActionReceiverBridge : MonoBehaviour, INotificationReceiver
{
    private static Dictionary<Type, Action<Playable, INotification, object, TimelineAction>> _actionHandler;
    private static Dictionary<Type, Action<Playable, INotification, object, TimelineAction>> actionHandler =>
        _actionHandler ??= new Dictionary<Type, Action<Playable, INotification, object, TimelineAction>>
    {
        { typeof(ActionLockMarker), LockAction },
        { typeof(MovementLockMarker), LockMovement },
        { typeof(ActionUnLockMarker), UnLockAction },
        { typeof(MovementUnLockMarker), UnLockMovement },
        { typeof(RecycleGameobjectMarker), RecycleGameobject },
        { typeof(InterruptOldActionMarker), InterruptOldAction },
        { typeof(PlayAudioMarker), PlayAudio }
    };
    
    private TimelineAction timelineAction;
    
    private void Awake()
    {
        timelineAction = GetComponent<TimelineAction>();
    }

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (actionHandler.TryGetValue(notification.GetType(), out var handler))
        {
            handler(origin, notification, context, timelineAction);
        }
    }

    private static void LockAction(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is ActionLockMarker alm)
            timelineAction.ActionControl.SetActionLocked(timelineAction, true);
    }
    
    private static void LockMovement(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is MovementLockMarker mlm)
            timelineAction.ActionControl.SetMovementLocked(timelineAction, true);
    }
    
    private static void UnLockAction(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is ActionUnLockMarker alm)
            timelineAction.ActionControl.SetActionLocked(timelineAction, false);
    }
    
    private static void UnLockMovement(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is MovementUnLockMarker mlm)
            timelineAction.ActionControl.SetMovementLocked(timelineAction, false);
    }

    private static void RecycleGameobject(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is RecycleGameobjectMarker rgm)
            timelineAction.ActionControl.RecycleGameObject(timelineAction);
    }
    
    private static void InterruptOldAction(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if(notification is InterruptOldActionMarker rom)
            timelineAction.ActionControl.RemoveOldAction();
    }

    private static void PlayAudio(Playable origin, INotification notification, object context, TimelineAction timelineAction)
    {
        if (notification is PlayAudioMarker pam)
            timelineAction.ActionControl.PlayAudio(pam.AudioDefinition, timelineAction);
    }
}
