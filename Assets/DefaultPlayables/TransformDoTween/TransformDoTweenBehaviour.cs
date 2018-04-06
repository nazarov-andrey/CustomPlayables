using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class TransformDoTweenBehaviour : PlayableBehaviour
{
#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (TransformDoTweenBehaviour))]
    public class TransformDoTweenDrawer : PropertyDrawer
    {
        private Rect DrawProperty (SerializedProperty property, Rect rect)
        {
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight (property);
            EditorGUI.PropertyField (rect, property);

            return rect;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty startLocationProp = property.FindPropertyRelative ("startPosition");
            SerializedProperty endLocationProp = property.FindPropertyRelative ("endPosition");
            SerializedProperty startRotationProp = property.FindPropertyRelative ("startRotation");
            SerializedProperty endRotationProp = property.FindPropertyRelative ("endRotation");
            SerializedProperty positionEaseProp = property.FindPropertyRelative ("positionEase");
            SerializedProperty rotationEaseProp = property.FindPropertyRelative ("rotationEase");

            Rect rect = new Rect (position.x, position.y, position.width, 0f);
            rect = DrawProperty (startLocationProp, rect);
            rect = DrawProperty (startRotationProp, rect);
            rect = DrawProperty (endLocationProp, rect);
            rect = DrawProperty (endRotationProp, rect);
            rect = DrawProperty (positionEaseProp, rect);
            DrawProperty (rotationEaseProp, rect);
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 9f;
        }
    }
#endif

    public Vector3 startPosition;
    public Vector3 startRotation;
    public Vector3 endPosition;
    public Vector3 endRotation;
    public Ease positionEase;
    public Ease rotationEase;
}