using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraControlClip : PlayableAsset, ITimelineClipAsset
{
    public CameraControlBehaviour template = new CameraControlBehaviour ();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraControlBehaviour>.Create (graph, template);
        return playable;
    }
}
