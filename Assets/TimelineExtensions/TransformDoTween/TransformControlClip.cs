using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TransformControlClip : PlayableAsset, ITimelineClipAsset
{
    public TransformControlBehaviour template = new TransformControlBehaviour ();

    public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.Extrapolation;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<BaseTransformBehaviour>.Create (graph, template);
        return playable;
    }
}