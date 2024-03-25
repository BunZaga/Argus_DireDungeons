using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, DisplayName("Interrupt Old Action Marker")]
public class InterruptOldActionMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}
