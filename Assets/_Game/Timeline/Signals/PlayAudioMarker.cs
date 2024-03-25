using System;
using System.ComponentModel;
using FunZaga.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable, CustomStyle("PlayAudio"), DisplayName("Play Audio Marker")]
public class PlayAudioMarker: Marker, INotification
{
    public AudioDefinition AudioDefinition => audioDefinition;
    [SerializeField] private AudioDefinition audioDefinition;
    public PropertyName id => new PropertyName();
}