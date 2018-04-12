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
            DrawProperty (rotationProp, rect);

            Rect gearPos = position;
            gearPos.x = gearPos.width;
            if (Utils.GearButton (gearPos)) {
                GenericMenu menu = new GenericMenu ();
                menu.AddItem (
                    new GUIContent ("Copy"),
                    false,
                    () => Utils.SerializeToSystemCopyBuffer (new PositionRotationPair (positionProp.vector3Value,
                        rotationProp.vector3Value)));

                if (Utils.SystemBufferContains<PositionRotationPair> ())
                    menu.AddItem (
                        new GUIContent ("Paste"),
                        false,
                        () =>
                        {
                            PositionRotationPair positionRotationPair;
                            if (Utils.DeserializeFromSystemCopyBuffer (out positionRotationPair)) {
                                positionProp.vector3Value = positionRotationPair.Position;
                                rotationProp.vector3Value = positionRotationPair.Rotation;

                                Utils.ApplyModificationsAndRebuildTimelineGraph (property.serializedObject);
                            }
                        });
                else
                    menu.AddDisabledItem (new GUIContent ("Paste"));

                menu.ShowAsContext ();
            }
        }
    }
#endif

    public Vector3 Position;
    public Vector3 Rotation;
}