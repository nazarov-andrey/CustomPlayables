using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class CameraDoTweenBehaviour : PlayableBehaviour
{
#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (CameraDoTweenBehaviour))]
    public class CameraControlDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return 3f * EditorGUIUtility.singleLineHeight;
        }

        private Rect DrawProperty (SerializedProperty property, Rect rect)
        {
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight (property);
            EditorGUI.PropertyField (rect, property);

            return rect;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty startFieldOfViewProp = property.FindPropertyRelative ("startFieldOfView");
            SerializedProperty endFieldOfViewProp = property.FindPropertyRelative ("endFieldOfView");
            SerializedProperty easeProp = property.FindPropertyRelative ("ease");

            Rect rect = new Rect (position.x, position.y, position.width, 0);
            rect = DrawProperty (startFieldOfViewProp, rect);
            rect = DrawProperty (endFieldOfViewProp, rect);
            DrawProperty (easeProp, rect);
        }
    }
#endif

    [SerializeField]
    private float startFieldOfView;

    [SerializeField]
    private float endFieldOfView;

    [SerializeField]
    private Ease ease;

    public CameraBehaviourOutput Evaluate (double time, double duration)
    {
        var fduration = (float) duration;
        var ftime = (float) time;
        float FOW = 0f;

        DOTween
            .To (() => startFieldOfView, x => FOW = x, endFieldOfView, fduration)
            .SetEase (ease)
            .Goto (ftime);

        return new CameraBehaviourOutput (FOW);
    }
}