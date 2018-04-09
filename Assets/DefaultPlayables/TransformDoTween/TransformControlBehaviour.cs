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

            float width = 50f;
            float space = 10f;
            rect.x = rect.x + (rect.width - 2 * width - space) / 2f;
            rect.y += rect.height;
            rect.width = width;
            rect.height = EditorGUIUtility.singleLineHeight;
            if (GUI.Button (rect, "Copy")) {
                Utils.SerializeToSystemCopyBuffer (new PositionRotationPair (positionProp.vector3Value, rotationProp.vector3Value));
            }

            GUI.enabled = Utils.SystemBufferContains<PositionRotationPair> ();
            rect.x += space + width;
            if (GUI.Button (rect, "Paste")) {
                PositionRotationPair positionRotationPair;
                if (Utils.DeserializeFromSystemCopyBuffer (out positionRotationPair)) {
                    positionProp.vector3Value = positionRotationPair.Position;
                    rotationProp.vector3Value = positionRotationPair.Rotation;
                }
            }

            GUI.enabled = true;
        }
    }
#endif

    public Vector3 Position;
    public Vector3 Rotation;
}