using System;
using TimelineExtensions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class TransformDoTweenClip : PlayableAsset, ITimelineClipAsset
{
#if UNITY_EDITOR
    [CustomEditor (typeof (TransformDoTweenClip))]
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
            SerializedProperty startPosProp = templateProp.FindPropertyRelative ("startPosition");
            SerializedProperty endPosProp = templateProp.FindPropertyRelative ("endPosition");
            SerializedProperty startRotProp = templateProp.FindPropertyRelative ("startRotation");
            SerializedProperty endRotProp = templateProp.FindPropertyRelative ("endRotation");

            Vector3 startPos = startPosProp.vector3Value;
            Vector3 endPos = endPosProp.vector3Value;
            Quaternion startRot = Quaternion.Euler (startRotProp.vector3Value);
            Quaternion endRot = Quaternion.Euler (endRotProp.vector3Value);

            Vector3 newStartPos = Handles.PositionHandle (startPos, startRot);
            Vector3 newEndPos = Handles.PositionHandle (endPos, endRot);
            Quaternion newStartRot = Handles.RotationHandle (startRot, startPos);
            Quaternion newEndRot = Handles.RotationHandle (endRot, endPos);

            Handles.Label (startPos + new Vector3 (0f, 2f, 0f), "Start Position");
            Handles.Label (endPos + new Vector3 (0f, 2f, 0f), "End Position");

            bool hasChanges = false;
            if (startPos != newStartPos) {
                hasChanges = true;
                startPosProp.vector3Value = newStartPos;
            }

            if (endPos != newEndPos) {
                hasChanges = true;
                endPosProp.vector3Value = newEndPos;
            }

            if (startRot != newStartRot) {
                hasChanges = true;
                startRotProp.vector3Value = newStartRot.eulerAngles;
            }

            if (endRot != newEndRot) {
                hasChanges = true;
                endRotProp.vector3Value = newEndRot.eulerAngles;
            }

            if (hasChanges) {
                serializedObject.ApplyModifiedProperties ();
                Utils.RebuildTimelineGraph ();

                Repaint ();
            }
        }
    }
#endif

    public TransformDoTweenBehaviour template = new TransformDoTweenBehaviour ();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TransformDoTweenBehaviour>.Create (graph, template);
        return playable;
    }
}