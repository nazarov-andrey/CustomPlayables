using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public struct PositionRotationPair
{
    public readonly Vector3 Position;
    public readonly Vector3 Rotation;

    public PositionRotationPair (Vector3 position, Vector3 rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

public class BaseTransformBehaviour : PlayableBehaviour
{
    public virtual PositionRotationPair Evaluate (double time, double duration)
    {
        throw new NotImplementedException ();
    }

    public virtual Space Space {
        get {
            throw new NotImplementedException ();
        }
    }
}
