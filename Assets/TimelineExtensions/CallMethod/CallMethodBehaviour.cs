using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor;

#endif

[Serializable]
public class CallMethodBehaviour : PlayableBehaviour
{
    [HideInInspector]
    public Component Component;

    [HideInInspector]
    public string MethodName;

    [HideInInspector]
    public bool RunInEditorMode;

    public override void OnBehaviourPlay (Playable playable, FrameData info)
    {
        base.OnBehaviourPlay (playable, info);
        
        if (Component == null)
            return;

        if (!Application.isPlaying && !RunInEditorMode) {
            Debug.Log ("Run method '" + MethodName + "' of object '" + Component.name + "'", Component.gameObject);
        } else {
            MethodInfo methodInfo = Component
                .GetType ()
                .GetMethod (MethodName);

            if (methodInfo != null)
                methodInfo.Invoke (Component, new object[0]);
        }
    }
}