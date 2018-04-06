using System;
using UnityEditor;

namespace TimelineExtensions
{
    public static class Utils
    {
        public static void SerializeToSystemCopyBuffer (object o)
        {
            EditorGUIUtility.systemCopyBuffer = o.GetType () + " " + EditorJsonUtility.ToJson (o);
        }

        public static bool DeserializeFromSystemCopyBuffer<T> (out T o) where T : new()
        {
            o = default (T);
            string type = typeof (T).ToString ();
            if (!EditorGUIUtility.systemCopyBuffer.StartsWith (type))
                return false;

            o = new T ();
            EditorJsonUtility.FromJsonOverwrite (EditorGUIUtility.systemCopyBuffer.Replace(type, ""), o);
            
            return true;
        }
    }
}