#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimelineExtensions
{
    public static class PlayableDirectorMenuItems
    {
        class TrackComparer : IEqualityComparer<TrackAsset>
        {
            public bool Equals (TrackAsset x, TrackAsset y)
            {
                if (x == null && y == null)
                    return true;

                if (x == null || y == null)
                    return false;

                return x.GetType () == y.GetType () && x.name == y.name;
            }

            public int GetHashCode (TrackAsset x)
            {
                unchecked {
                    var hashCode = x.GetType ().GetHashCode ();
                    hashCode = (hashCode * 397) ^ (string.IsNullOrEmpty (x.name) ? 0 : x.name.GetHashCode ());
                    return hashCode;
                }
            }

            private static TrackComparer instance;

            public static TrackComparer Instance {
                get {
                    if (instance == null)
                        instance = new TrackComparer ();

                    return instance;
                }
            }
        }

        struct TrackBinding
        {
            public TrackAsset Track;
            public Object Binding;
            public SerializedProperty Property;

            public TrackBinding (TrackAsset track, Object binding, SerializedProperty property)
            {
                Track = track;
                Binding = binding;
                Property = property;
            }
        }

        private static PlayableDirector copySource;

        private static TrackAsset GetKeyAsTrackAsset (SerializedProperty serializedProperty)
        {
            return serializedProperty
                .FindPropertyRelative ("key")
                .objectReferenceValue as TrackAsset;
        }

        private static SerializedProperty GetValueAsProperty (SerializedProperty serializedProperty)
        {
            return serializedProperty.FindPropertyRelative ("value");
        }

        private static Object GetValueAsObject (SerializedProperty serializedProperty)
        {
            return GetValueAsProperty (serializedProperty).objectReferenceValue;
        }

        private static Object SetValueAsObject (SerializedProperty serializedProperty, Object o)
        {
            return GetValueAsProperty (serializedProperty).objectReferenceValue = o;
        }

        private static IEnumerable<TrackBinding> GetTrackBinding (SerializedProperty bindings)
        {
            var trackBindings = new List<TrackBinding> ();
            for (var i = 0; i < bindings.arraySize; i++) {
                SerializedProperty binding = bindings.GetArrayElementAtIndex (i);
                TrackAsset track = GetKeyAsTrackAsset (binding);
                if (track == null)
                    continue;

                trackBindings.Add (new TrackBinding (track, GetValueAsObject (binding), binding));
            }

            return trackBindings;
        }

        [MenuItem ("CONTEXT/PlayableDirector/Copy Bindings")]
        public static void CopyBindings (MenuCommand menuCommand)
        {
            copySource = menuCommand.context as PlayableDirector;
        }

        [MenuItem ("CONTEXT/PlayableDirector/Paste Bindings")]
        public static void PasteBindings (MenuCommand menuCommand)
        {
            SerializedObject src = new SerializedObject (copySource);
            SerializedObject dst = new SerializedObject (menuCommand.context);

            SerializedProperty srcBindings = src.FindProperty ("m_SceneBindings");
            SerializedProperty dstBindings = dst.FindProperty ("m_SceneBindings");

            IEnumerable<TrackBinding> srcTrackBindings = GetTrackBinding (srcBindings);
            IEnumerable<TrackBinding> dstTrackBindings = GetTrackBinding (dstBindings);

            dstTrackBindings
                .Join (
                    srcTrackBindings,
                    x => x.Track,
                    x => x.Track,
                    (d, s) => SetValueAsObject (d.Property, s.Binding),
                    TrackComparer.Instance)
                .ToArray ();

            dst.ApplyModifiedProperties ();
        }

        [MenuItem ("CONTEXT/PlayableDirector/Paste Bindings", true)]
        public static bool ValidatePasteBindings (MenuCommand menuCommand)
        {
            return copySource != null;
        }
    }
}
#endif