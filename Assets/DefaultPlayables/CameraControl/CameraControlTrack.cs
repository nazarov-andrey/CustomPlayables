using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CameraControlClip))]
[TrackBindingType(typeof(Camera))]
public class CameraControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<CameraControlMixerBehaviour>.Create (graph, inputCount);
    }

    // Please note this assumes only one component of type Camera on the same gameobject.
    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        Camera trackBinding = director.GetGenericBinding(this) as Camera;
        if (trackBinding == null)
            return;

        var serializedObject = new UnityEditor.SerializedObject (trackBinding);
        var iterator = serializedObject.GetIterator();
        while (iterator.NextVisible(true))
        {
            if (iterator.hasVisibleChildren)
                continue;

            driver.AddFromName<Camera>(trackBinding.gameObject, iterator.propertyPath);
        }
#endif
        base.GatherProperties (director, driver);
    }
}
