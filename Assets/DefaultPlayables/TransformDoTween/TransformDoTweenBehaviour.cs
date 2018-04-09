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
#if UNITY_EDITOR
    [CustomPropertyDrawer (typeof (TransformDoTweenBehaviour))]
    public class TransformDoTweenDrawer : PropertyDrawer
    {
        private GenericMenu GetMenu (
            GenericMenu.MenuFunction startFunc,
            GenericMenu.MenuFunction endFunc)
        {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("Start"), false, startFunc);
            menu.AddItem (new GUIContent ("End"), false, endFunc);

            return menu;
        }

        private void Paste (SerializedProperty property, string positionProperty, string rotationProperty)
        {
            PositionRotationPair positionRotationPair;
            if (!Utils.DeserializeFromSystemCopyBuffer (out positionRotationPair))
                return;

            property
                .FindPropertyRelative (positionProperty)
                .vector3Value = positionRotationPair.Position;

            property
                .FindPropertyRelative (rotationProperty)
                .vector3Value = positionRotationPair.Rotation;

            property.serializedObject.ApplyModifiedProperties ();
            property.serializedObject.Update ();
            Utils.RebuildTimelineGraph ();
        }

        private void Copy (SerializedProperty position, SerializedProperty rotation)
        {
            Utils.SerializeToSystemCopyBuffer (new PositionRotationPair (position.vector3Value, rotation.vector3Value));
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

            float width = 50f;
            float space = 10f;
            rect.x = rect.x + (rect.width - 2 * width - space) / 2f;
            rect.y += rect.height;
            rect.width = width;
            if (GUI.Button (rect, "Copy")) {
                GenericMenu menu = GetMenu (() => Copy (startPositionProp, startRotationProp), () =>
                    Copy (endPositionProp, endRotationProp));
                menu.ShowAsContext ();
            }

            GUI.enabled = Utils.SystemBufferContains<PositionRotationPair> ();
            rect.x += space + width;
            if (GUI.Button (rect, "Paste")) {
                GenericMenu menu = GetMenu (() => Paste (property, "startPosition", "startRotation"), () =>
                    Paste (property, "endPosition", "endRotation"));
                menu.ShowAsContext ();
            }

            GUI.enabled = true;
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