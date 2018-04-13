using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class CameraDoTweenClip : PlayableAsset, ITimelineClipAsset
{
    public CameraDoTweenBehaviour template = new CameraDoTweenBehaviour ();

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraDoTweenBehaviour>.Create (graph, template);
        return playable;
    }
}
