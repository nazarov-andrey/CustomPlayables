using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using DG.Tweening;
using TimelineExtensions;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class TransformDoTweenBehaviour : PlayableBehaviour
{
    [Serializable]
    public class PositionRotationPair
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public PositionRotationPair ()
        {
        }

        public PositionRotationPair (Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public override string ToString ()
        {
            return $"PositionRotationPair {nameof (Position)}: {Position}, {nameof (Rotation)}: {Rotation}";
        }
    }

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
            SerializedProperty startPositionProp = property.FindPropertyRelative ("startPosition");
            SerializedProperty endPositionProp = property.FindPropertyRelative ("endPosition");
            SerializedProperty startRotationProp = property.FindPropertyRelative ("startRotation");
            SerializedProperty endRotationProp = property.FindPropertyRelative ("endRotation");
            SerializedProperty positionEaseProp = property.FindPropertyRelative ("positionEase");
            SerializedProperty rotationEaseProp = property.FindPropertyRelative ("rotationEase");

            Rect rect = new Rect (position.x, position.y, position.width, 0f);
            rect = DrawProperty (startPositionProp, rect);
            rect = DrawProperty (startRotationProp, rect);
            rect = DrawProperty (endPositionProp, rect);
            rect = DrawProperty (endRotationProp, rect);
            rect = DrawProperty (positionEaseProp, rect);
            rect = DrawProperty (rotationEaseProp, rect);

            float width = 90f;
            rect.x = rect.x + (rect.width - width) / 2f;
            rect.y += rect.height;
            rect.width = width;
            if (GUI.Button (rect, "Copy End"))
                Utils.SerializeToSystemCopyBuffer (
                    new PositionRotationPair (endPositionProp.vector3Value, endRotationProp.vector3Value));
        }

        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 10f;
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