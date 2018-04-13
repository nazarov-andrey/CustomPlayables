using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public struct TransformBehaviourOutput
{
    public readonly Vector3 Position;
    public readonly Vector3 Rotation;

    public TransformBehaviourOutput (Vector3 position, Vector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public class BaseTransformBehaviour : PlayableBehaviour
{
    public virtual TransformBehaviourOutput Evaluate (double time, double duration)
    {
        throw new NotImplementedException ();
    }

    public virtual Space Space {
        get {
            throw new NotImplementedException ();
        }
    }
}
