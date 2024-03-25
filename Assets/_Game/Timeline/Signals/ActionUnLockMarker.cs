using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, CustomStyle("ActionUnLock"), DisplayName("Action UnLock Marker")]
public class ActionUnLockMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}