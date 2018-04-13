using System;
using DG.Tweening;
using TimelineExtensions;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class TransformControlBehaviour : BaseTransformBehaviour
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
            SerializedProperty positionProp = property.FindPropertyRelative ("position");
            SerializedProperty rotationProp = property.FindPropertyRelative ("rotation");
            SerializedProperty spaceProp = property.FindPropertyRelative ("space");

            Rect rect = new Rect (position.x, position.y, position.width, 0f);
            rect = DrawProperty (positionProp, rect);
            rect = DrawProperty (rotationProp, rect);
            rect = DrawProperty (spaceProp, rect);

            Rect gearPos = position;
            gearPos.x = gearPos.width;
            gearPos.y -= EditorGUIUtility.singleLineHeight;
            if (Utils.GearButton (gearPos)) {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem (
                    new GUIContent ("Copy"),
                    false,
                    () => Utils.CopyToPasteboardAsTransform (positionProp.vector3Value, rotationProp.vector3Value));

                if (Utils.PasteboardContainsTransform ())
                    menu.AddItem (
                        new GUIContent ("Paste"),
                        false,
                        () =>
                        {
                            Utils.PasteFromPasteboardTransform (positionProp, rotationProp);
                            Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
                        });
                else
                    menu.AddDisabledItem (new GUIContent ("Paste"));

                menu.ShowAsContext ();
            }
        }
    }
#endif

    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private Vector3 rotation;

    [SerializeField]
    private Space space = Space.World;

    public override TransformBehaviourOutput Evaluate (double time, double duration)
    {
        return new TransformBehaviourOutput (position, rotation);
    }

    public override Space Space => space;
}