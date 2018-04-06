using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class CameraControlBehaviour : PlayableBehaviour
{
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(CameraControlBehaviour))]
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
            SerializedProperty startFieldOfViewProp = property.FindPropertyRelative("StartFieldOfView");
            SerializedProperty endFieldOfViewProp = property.FindPropertyRelative("EndFieldOfView");
            SerializedProperty easeProp = property.FindPropertyRelative("Ease");

            Rect rect = new Rect(position.x, position.y, position.width, 0);
            rect = DrawProperty (startFieldOfViewProp, rect);
            rect = DrawProperty (endFieldOfViewProp, rect);
            DrawProperty (easeProp, rect);
        }
    }    
    #endif
    
    public float StartFieldOfView;
    public float EndFieldOfView;
    public Ease Ease;
}
