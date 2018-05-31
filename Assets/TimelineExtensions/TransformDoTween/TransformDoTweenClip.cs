using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using TimelineExtensions;
#endif

[Serializable]
public class TransformDoTweenClip : PlayableAsset, ITimelineClipAsset
{
#if UNITY_EDITOR
    [CustomEditor (typeof (TransformDoTweenClip)), DisallowMultipleComponent]
    public class TransformDoTweenClipEditor : Editor
    {
        private void OnDestroy ()
        {
            UnlistenSceneGUI ();
        }

        private void OnDisable ()
        {
            UnlistenSceneGUI ();
        }

        private void UnlistenSceneGUI ()
        {
            if (SceneView.onSceneGUIDelegate != null)
                SceneView.onSceneGUIDelegate -= OnSceneGui;
        }

        private void OnEnable ()
        {
            SceneView.onSceneGUIDelegate += OnSceneGui;
        }

        private void OnSceneGui (SceneView sceneView)
        {
            SerializedProperty templateProp = serializedObject.FindProperty ("template");
            SerializedProperty startPosProp =
                templateProp.FindPropertyRelative (TransformDoTweenBehaviour.StartPositionPropName);
            SerializedProperty endPosProp =
                templateProp.FindPropertyRelative (TransformDoTweenBehaviour.EndPositionPropName);
            SerializedProperty startRotProp =
                templateProp.FindPropertyRelative (TransformDoTweenBehaviour.StartRotationPropName);
            SerializedProperty endRotProp =
                templateProp.FindPropertyRelative (TransformDoTweenBehaviour.EndRotationPropName);

            var space = (Space) templateProp
                .FindPropertyRelative (TransformDoTweenBehaviour.SpacePropName)
                .enumValueIndex;

            Transform trackBinding = null;
            if (TimelineEditor.playableDirector != null) {
                var playableDirector = new SerializedObject (TimelineEditor.playableDirector);
                var bindings = playableDirector.FindProperty ("m_SceneBindings");
                for (int i = 0; i < bindings.arraySize; i++) {
                    var binding = bindings.GetArrayElementAtIndex (i);
                    var track = binding.FindPropertyRelative ("key").objectReferenceValue as TrackAsset;
                    if (track == null)
                        continue;

                    bool isClipFromTrack = track
                        .GetClips ()
                        .Any (x => x.asset == serializedObject.targetObject);

                    if (isClipFromTrack) {
                        trackBinding = binding
                            .FindPropertyRelative ("value")
                            .objectReferenceValue as Transform;

                        break;
                    }
                }
            }

            Vector3 startPos = startPosProp.vector3Value;
            Vector3 endPos = endPosProp.vector3Value;
            Quaternion startRot = Quaternion.Euler (startRotProp.vector3Value);
            Quaternion endRot = Quaternion.Euler (endRotProp.vector3Value);
            bool localSpace = trackBinding != null && trackBinding.parent != null && space == Space.Self;

            if (localSpace) {
                startPos = trackBinding.parent.TransformPoint (startPos);
                endPos = trackBinding.parent.TransformPoint (endPos);
                startRot = trackBinding.parent.rotation * startRot;
                endRot = trackBinding.parent.rotation * endRot;
            }

            Vector3 newStartPos = Handles.PositionHandle (startPos, startRot);
            Vector3 newEndPos = Handles.PositionHandle (endPos, endRot);
            Quaternion newStartRot = Handles.RotationHandle (startRot, startPos);
            Quaternion newEndRot = Handles.RotationHandle (endRot, endPos);

            Handles.Label (startPos + new Vector3 (0f, 2f, 0f), "Start Position");
            Handles.Label (endPos + new Vector3 (0f, 2f, 0f), "End Position");

            bool hasChanges = startPos != newStartPos
                              || endPos != newEndPos
                              || startRot != newStartRot
                              || endRot != newEndRot;

            if (hasChanges) {
                if (localSpace) {
                    newStartPos = trackBinding.parent.InverseTransformPoint (newStartPos);
                    newEndPos = trackBinding.parent.InverseTransformPoint (newEndPos);

                    newStartRot = Quaternion.Inverse (trackBinding.parent.rotation) * newStartRot;
                    newEndRot = Quaternion.Inverse (trackBinding.parent.rotation) * newEndRot;
                }

                startPosProp.vector3Value = newStartPos;
                endPosProp.vector3Value = newEndPos;
                startRotProp.vector3Value = newStartRot.eulerAngles;
                endRotProp.vector3Value = newEndRot.eulerAngles;

                Utils.ApplyModificationsAndRebuildTimelineGraph (serializedObject);
                Repaint ();
            }
        }
    }
#endif

    public TransformDoTweenBehaviour template = new TransformDoTweenBehaviour ();

    public ClipCaps clipCaps => ClipCaps.Blending | ClipCaps.Extrapolation;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformDoTweenBehaviour>.Create (graph, template);
        return playable;
    }
}