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
public class TransformDoTweenBehaviour : BaseTransformBehaviour
{
    public const string StartPositionPropName = "startPosition";
    public const string EndPositionPropName = "endPosition";
    public const string StartRotationPropName = "startRotation";
    public const string EndRotationPropName = "endRotation";
    public const string SpacePropName = "space";
    public const string PositionEasePropName = "positionEase";
    public const string RotationEasePropName = "rotationEase";
    
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

        private void Reset (SerializedProperty property, string positionProp, string rotationProp, bool apply = true)
        {
            property
                .FindPropertyRelative (positionProp)
                .vector3Value = Vector3.zero;

            property
                .FindPropertyRelative (rotationProp)
                .vector3Value = Vector3.zero;

            if (apply)
                Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
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
            SwapVector3Properties (property.FindPropertyRelative (StartPositionPropName),
                property.FindPropertyRelative (EndPositionPropName));
            SwapVector3Properties (property.FindPropertyRelative (StartRotationPropName),
                property.FindPropertyRelative (EndRotationPropName));

            Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty startPositionProp = property.FindPropertyRelative (StartPositionPropName);
            SerializedProperty endPositionProp = property.FindPropertyRelative (EndPositionPropName);
            SerializedProperty startRotationProp = property.FindPropertyRelative (StartRotationPropName);
            SerializedProperty endRotationProp = property.FindPropertyRelative (EndRotationPropName);
            SerializedProperty spaceProp = property.FindPropertyRelative (SpacePropName);
            SerializedProperty positionEaseProp = property.FindPropertyRelative (PositionEasePropName);
            SerializedProperty rotationEaseProp = property.FindPropertyRelative (RotationEasePropName);

            Rect rect = new Rect (position.x, position.y, position.width, 0f);
            rect = DrawProperty (startPositionProp, rect);
            rect = DrawProperty (startRotationProp, rect);
            rect = DrawProperty (endPositionProp, rect);
            rect = DrawProperty (endRotationProp, rect);
            rect = DrawProperty (spaceProp, rect);
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

                menu.AddItem (
                    new GUIContent ("Reset/Start"),
                    false,
                    () => Reset (property, StartPositionPropName, StartRotationPropName));

                menu.AddItem (
                    new GUIContent ("Reset/End"),
                    false,
                    () => Reset (property, EndPositionPropName, EndRotationPropName));
                
                menu.AddItem (
                    new GUIContent ("Reset/All"),
                    false,
                    () =>
                    {
                        Reset (property, StartPositionPropName, StartRotationPropName, false);
                        Reset (property, EndPositionPropName, EndRotationPropName);
                    });

                if (Utils.PasteboardContainsTransform ()) {
                    menu.AddItem (
                        new GUIContent ("Paste/Start"),
                        false,
                        () => Paste (property, StartPositionPropName, StartRotationPropName));

                    menu.AddItem (
                        new GUIContent ("Paste/End"),
                        false,
                        () => Paste (property, EndPositionPropName, EndRotationPropName));
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

    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private Vector3 startRotation;

    [SerializeField]
    private Vector3 endPosition;

    [SerializeField]
    private Vector3 endRotation;

    [SerializeField]
    private Space space = Space.World;

    [SerializeField]
    private Ease positionEase;

    [SerializeField]
    private Ease rotationEase;

    private Tweener positionTweener;
    private Tweener rotationTweener;

    public override TransformBehaviourOutput Evaluate (double time, double duration)
    {
        var fduration = (float) duration;
        var ftime = (float) time;
        Vector3 position = Vector3.zero, rotation = Vector3.zero;

        DOTween
            .To (() => startPosition, x => position = x, endPosition, fduration)
            .SetEase (positionEase)
            .Goto (ftime);

        DOTween
            .To (() => startRotation, x => rotation = x, endRotation, fduration)
            .SetEase (rotationEase)
            .Goto (ftime);

        return new TransformBehaviourOutput (position, rotation);
    }

    public override Space Space => space;
}