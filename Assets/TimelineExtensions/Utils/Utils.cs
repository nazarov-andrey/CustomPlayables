#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Timeline;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TimelineExtensions
{
    public static class Utils
    {
        public static void CopyToPasteboardAsTransform (Vector3 position, Vector3 rotation)
        {
            GameObject gameObject = new GameObject ();
            gameObject.transform.position = position;
            gameObject.transform.rotation = Quaternion.Euler (rotation);

            ComponentUtility.CopyComponent (gameObject.transform);
            Object.DestroyImmediate (gameObject);
        }

        public static bool PasteboardContainsTransform ()
        {
            GameObject gameObject = new GameObject ();
            bool result = ComponentUtility.PasteComponentValues (gameObject.transform);
            Object.DestroyImmediate (gameObject);

            return result;
        }

        public static void PasteFromPasteboardTransform (SerializedProperty position, SerializedProperty rotatation)
        {
            GameObject gameObject = new GameObject ();
            if (ComponentUtility.PasteComponentValues (gameObject.transform)) {
                position.vector3Value = gameObject.transform.position;
                rotatation.vector3Value = gameObject.transform.rotation.eulerAngles;
            }

            Object.DestroyImmediate (gameObject);
        }

        public static void RebuildTimelineGraph ()
        {
            if (TimelineEditor.playableDirector != null) {
                TimelineEditor.playableDirector.RebuildGraph ();
                TimelineEditor.playableDirector.Evaluate ();                
            }
        }

        public static bool GearButton (Rect rect)
        {
            GUIStyle iconButtonStyle = GUI.skin.FindStyle ("IconButton") ??
                                       EditorGUIUtility.GetBuiltinSkin (EditorSkin.Inspector).FindStyle ("IconButton");
            GUIContent content = new GUIContent (EditorGUIUtility.Load ("icons/_Popup.png") as Texture2D);

            return EditorGUI.DropdownButton (rect, content, FocusType.Passive, iconButtonStyle);
        }

        public static void ApplyModificationsAndRebuildTimelineGraph (SerializedObject serializedObject)
        {
            serializedObject.ApplyModifiedProperties ();
            serializedObject.Update ();
            RebuildTimelineGraph ();
        }
    }
}
#endif