#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;

namespace TimelineExtensions
{
    public static class Utils
    {
        public static void SerializeToSystemCopyBuffer (object o)
        {
            EditorGUIUtility.systemCopyBuffer = o.GetType () + " " + EditorJsonUtility.ToJson (o);
        }

        public static bool SystemBufferContains<T> ()
        {
            return EditorGUIUtility.systemCopyBuffer.StartsWith (typeof (T).ToString ());
        }

        public static bool DeserializeFromSystemCopyBuffer<T> (out T o) where T : new ()
        {
            o = default (T);
            string type = typeof (T).ToString ();
            if (!EditorGUIUtility.systemCopyBuffer.StartsWith (type))
                return false;

            o = new T ();
            EditorJsonUtility.FromJsonOverwrite (EditorGUIUtility.systemCopyBuffer.Replace (type, ""), o);

            return true;
        }

        public static void RebuildTimelineGraph ()
        {
            TimelineEditor.playableDirector.RebuildGraph ();
            TimelineEditor.playableDirector.Evaluate ();
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