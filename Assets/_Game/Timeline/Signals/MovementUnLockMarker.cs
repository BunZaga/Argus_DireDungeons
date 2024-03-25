using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, CustomStyle("MovementUnLock"), DisplayName("Movement UnLock Marker")]
public class MovementUnLockMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}