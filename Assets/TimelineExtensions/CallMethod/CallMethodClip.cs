using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class CallMethodClip : PlayableAsset, ITimelineClipAsset
{
#if UNITY_EDITOR
    [CustomEditor (typeof (CallMethodClip))]
    public class CallMethodClipEditor : Editor
    {
        public override void OnInspectorGUI ()
        {
            base.OnInspectorGUI ();

            SerializedProperty componentProp = serializedObject.FindProperty ("Component");
            SerializedProperty methodNameProp = serializedObject.FindProperty ("MethodName");
            SerializedProperty runInEditorModeProp = serializedObject.FindProperty ("RunInEditorMode");

            UnityEngine.Object obj = componentProp.exposedReferenceValue;
            if (obj != null) {
                Type type = componentProp.exposedReferenceValue.GetType ();
                string[] methodInfos = type
                    .GetMethods (BindingFlags.Public | BindingFlags.Instance)
                    .Where (x => !x.IsSpecialName && x.GetParameters ().Length == 0)
                    .Select (x => x.Name)
                    .OrderBy (x => x)
                    .ToArray ();
                int index = Array.IndexOf (methodInfos, methodNameProp.stringValue);
                if (index < 0)
                    index = 0;
                else
                    index++;

                index = EditorGUILayout.Popup (
                    "Method",
                    index,
                    new[] {"<None>"}
                        .Concat (methodInfos.Select (x => x.Substring (0, 1) + "/" + x))
                        .ToArray ());

                methodNameProp.stringValue = index == 0 ? null : methodInfos[index - 1];                
            }
            

            EditorGUILayout.PropertyField (runInEditorModeProp);
            
            serializedObject.ApplyModifiedProperties ();
        }
    }
#endif

    public CallMethodBehaviour template = new CallMethodBehaviour ();
    public ExposedReference<Component> Component;

    [HideInInspector]
    public string MethodName;

    [HideInInspector]
    public bool RunInEditorMode;

    public ClipCaps clipCaps => ClipCaps.None;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CallMethodBehaviour>.Create (graph, template);
        CallMethodBehaviour clone = playable.GetBehaviour ();
        clone.Component = Component.Resolve (graph.GetResolver ());
        clone.MethodName = MethodName;
        clone.RunInEditorMode = RunInEditorMode;
        return playable;
    }
}