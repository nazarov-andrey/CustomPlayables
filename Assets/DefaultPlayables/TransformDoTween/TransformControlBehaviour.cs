using System;
using TimelineExtensions;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class TransformControlBehaviour : PlayableBehaviour
{
#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (TransformControlBehaviour))]
    public class TransformControlBehaviourPropertyDrawer : PropertyDrawer
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
            SerializedProperty positionProp = property.FindPropertyRelative ("Position");
            SerializedProperty rotationProp = property.FindPropertyRelative ("Rotation");

            Rect rect = new Rect (position.x, position.y, position.width, 0f);
            rect = DrawProperty (positionProp, rect);
            rect = DrawProperty (rotationProp, rect);

            TransformDoTweenBehaviour.PositionRotationPair positionRotationPair;
            if (Utils.DeserializeFromSystemCopyBuffer (out positionRotationPair)) {
                float width = 75f;
                rect.x = rect.x + (rect.width - width) / 2f;
                rect.y += rect.height;
                rect.width = width;
                rect.height = EditorGUIUtility.singleLineHeight;

                if (GUI.Button (rect, "Paste")) {
                    positionProp.vector3Value = positionRotationPair.Position;
                    rotationProp.vector3Value = positionRotationPair.Rotation;
                }
            }
        }
    }
#endif

    public Vector3 Position;
    public Vector3 Rotation;
}