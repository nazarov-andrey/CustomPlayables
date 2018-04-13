using System;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Win32.SafeHandles;
using UnityEngine;
using UnityEngine.Playables;

public class TransformMixerBehaviour : PlayableBehaviour
{
    private Vector3? defaultPosition;
    private Quaternion? defaultRotation;

    public override void ProcessFrame (Playable playable, FrameData info, object playerData)
    {
        Transform trackBinding = playerData as Transform;
        if (trackBinding == null)
            return;

        if (!defaultPosition.HasValue) {
            defaultPosition = trackBinding.position;
            defaultRotation = trackBinding.rotation;
        }

        Vector3 blendedPosition = Vector3.zero;
        Quaternion blendedRotation = Quaternion.identity;
        float totalWeight = 0f;
        Space? space = null;

        for (int i = 0, inputCount = playable.GetInputCount (); i < inputCount; i++) {
            Playable playableInput = playable.GetInput (i);
            float weight = playable.GetInputWeight (i);
            if (Mathf.Approximately (weight, 0f))
                continue;

            totalWeight += weight;

            ScriptPlayable<BaseTransformBehaviour> input = (ScriptPlayable<BaseTransformBehaviour>) playableInput;
            BaseTransformBehaviour transformBehaviour = input.GetBehaviour ();
            if (!space.HasValue)
                space = transformBehaviour.Space;
            else if (space.Value != transformBehaviour.Space)
                throw new Exception ("Cannot blend transform behaviours with different space");

            TransformBehaviourOutput transformBehaviourOutput = transformBehaviour.Evaluate (input.GetTime (), Durations[i]);
            blendedPosition += transformBehaviourOutput.Position * weight;
            blendedRotation *= ScaleQuaternion (Quaternion.Euler (transformBehaviourOutput.Rotation), weight);
        }

        blendedPosition += (1f - totalWeight) * defaultPosition.Value;
        blendedRotation *= ScaleQuaternion (defaultRotation.Value, 1f - totalWeight);

        if (space.HasValue && space.Value == Space.Self) {
            trackBinding.localPosition = blendedPosition;
            trackBinding.localRotation = blendedRotation;
        } else {
            trackBinding.position = blendedPosition;
            trackBinding.rotation = blendedRotation;
        }
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        defaultPosition = null;
        defaultRotation = null;
    }

    public double[] Durations;

    static Quaternion ScaleQuaternion (Quaternion rotation, float multiplier)
    {
        return Quaternion.Euler (rotation.eulerAngles * multiplier);
    }
}