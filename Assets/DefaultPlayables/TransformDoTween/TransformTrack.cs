using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.855f,0.8623f,0.870f)]
[TrackClipType(typeof(TransformDoTweenClip))]
[TrackClipType(typeof(TransformControlClip))]
[TrackBindingType(typeof(Transform))]
public class TransformTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
	    var playable = ScriptPlayable<TransformMixerBehaviour>.Create (graph, inputCount);
		playable.GetBehaviour().Durations = GetClips ()
	        .Select (x => x.duration)
	        .ToArray ();
	    
	    return playable;
	}

    public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
    {
#if UNITY_EDITOR
        var comp = director.GetGenericBinding(this) as Transform;
        if (comp == null)
            return;
        var so = new SerializedObject(comp);
        var iter = so.GetIterator();
        while (iter.NextVisible(true))
        {
            if (iter.hasVisibleChildren)
                continue;
            driver.AddFromName<Transform>(comp.gameObject, iter.propertyPath);
        }
#endif
        base.GatherProperties(director, driver);
    }
}
