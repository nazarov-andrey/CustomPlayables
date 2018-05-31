using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
#endif


[TrackColor(0.855f, 0.8623f, 0.87f)]
[TrackClipType(typeof(CameraDoTweenClip))]
[TrackBindingType(typeof(Camera))]
public class CameraTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var playable = ScriptPlayable<CameraMixerBehaviour>.Create (graph, inputCount);
        playable.GetBehaviour().Durations = GetClips ()
            .Select (x => x.duration)
            .ToArray ();
	    
        return playable;
    }

    // Please note this assumes only one component of type Camera on the same gameobject.
    public override void GatherProperties (PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        Camera trackBinding = director.GetGenericBinding(this) as Camera;
        if (trackBinding == null)
            return;

        var serializedObject = new SerializedObject (trackBinding);
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
