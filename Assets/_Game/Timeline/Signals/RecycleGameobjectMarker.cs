using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, DisplayName("Recycle Gameobject Marker")]
public class RecycleGameobjectMarker: Marker, INotification
{
    public PropertyName id => new PropertyName();
}