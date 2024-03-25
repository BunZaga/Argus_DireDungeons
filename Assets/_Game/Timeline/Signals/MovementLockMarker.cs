using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, CustomStyle("MovementLock"), DisplayName("Movement Lock Marker")]
public class MovementLockMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}