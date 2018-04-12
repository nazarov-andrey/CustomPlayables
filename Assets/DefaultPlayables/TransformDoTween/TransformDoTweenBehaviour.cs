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
        private void Paste (SerializedProperty property, string positionProperty, string rotationProperty)
        {
            Utils.PasteFromPasteboardTransform (
                property.FindPropertyRelative (positionProperty),
                property.FindPropertyRelative (rotationProperty));
            Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
        }

        private void Copy (SerializedProperty position, SerializedProperty rotation)
        {
            Utils.CopyToPasteboardAsTransform (position.vector3Value, rotation.vector3Value);
        }

        private Rect DrawProperty (SerializedProperty property, Rect rect)
        {
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight (property);
            EditorGUI.PropertyField (rect, property);

            return rect;
        }

        private void SwapVector3Properties (SerializedProperty a, SerializedProperty b)
        {
            Vector3 c = a.vector3Value;
            a.vector3Value = b.vector3Value;
            b.vector3Value = c;
        }

        private void Swap (SerializedProperty property)
        {
            SwapVector3Properties (property.FindPropertyRelative ("startPosition"),
                property.FindPropertyRelative ("endPosition"));
            SwapVector3Properties (property.FindPropertyRelative ("startRotation"),
                property.FindPropertyRelative ("endRotation"));

            Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
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

            Rect gearPos = position;
            gearPos.x = gearPos.width;
            gearPos.y -= EditorGUIUtility.singleLineHeight;
            if (Utils.GearButton (gearPos)) {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem (
                    new GUIContent ("Copy/Start"),
                    false,
                    () => Copy (startPositionProp, startRotationProp));

                menu.AddItem (
                    new GUIContent ("Copy/End"),
                    false,
                    () => Copy (endPositionProp, endRotationProp));

                if (Utils.PasteboardContainsTransform ()) {
                    menu.AddItem (
                        new GUIContent ("Paste/Start"),
                        false,
                        () => Paste (property, "startPosition", "startRotation"));

                    menu.AddItem (
                        new GUIContent ("Paste/End"),
                        false,
                        () => Paste (property, "endPosition", "endRotation"));
                } else {
                    menu.AddDisabledItem (new GUIContent ("Paste"));
                }

                menu.AddItem (new GUIContent ("Swap"), false, () => Swap (property));
                menu.ShowAsContext ();
            }
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