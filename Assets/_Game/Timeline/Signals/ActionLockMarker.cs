using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, CustomStyle("ActionLock"), DisplayName("Action Lock Marker")]
public class ActionLockMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}