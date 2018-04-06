using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class TransformDoTweenMixerBehaviour : PlayableBehaviour
{
    class VectorTweenProvider : ValueTweenProvider<Vector3, TransformDoTweenBehaviour>
    {
        protected override void RefreshTweener ()
        {
            TransformDoTweenBehaviour input = GetInput ();
            tweener = DOTween
                .To (() => value, x => value = x, input.endPosition, Duration)
                .SetEase (input.positionEase);
        }
    }

    class QuaternionTweenProvider : ValueTweenProvider<Quaternion, TransformDoTweenBehaviour>
    {
        protected override void RefreshTweener ()
        {
            TransformDoTweenBehaviour input = GetInput ();
            tweener = DOTween
                .To (() => value, x => value = x, input.endRotation, Duration)
                .SetEase (input.rotationEase);
        }
    }

    private Dictionary<ScriptPlayable<TransformDoTweenBehaviour>, VectorTweenProvider> positionProviders =
        new Dictionary<ScriptPlayable<TransformDoTweenBehaviour>, VectorTweenProvider> ();

    private Dictionary<ScriptPlayable<TransformDoTweenBehaviour>, QuaternionTweenProvider> rotationProviders =
        new Dictionary<ScriptPlayable<TransformDoTweenBehaviour>, QuaternionTweenProvider> ();

    private Vector3? defaultPosition;
    private Quaternion? defaultRotation;

    private TProvider GetValueProvider<TProvider, TType> (
        ScriptPlayable<TransformDoTweenBehaviour> playableInput,
        Dictionary<ScriptPlayable<TransformDoTweenBehaviour>, TProvider> providers,
        Func<TransformDoTweenBehaviour, TType> initialValueObtainer)
        where TProvider : ValueTweenProvider<TType, TransformDoTweenBehaviour>, new ()
        where TType : struct
    {
        TProvider valueTweenProvider;
        if (providers.TryGetValue (playableInput, out valueTweenProvider))
            return valueTweenProvider;

        valueTweenProvider = new TProvider ();
        valueTweenProvider.Init (playableInput, initialValueObtainer);

        providers.Add (playableInput, valueTweenProvider);

        return valueTweenProvider;
    }

    private VectorTweenProvider GetPositionProvider (ScriptPlayable<TransformDoTweenBehaviour> playableInput)
    {
        return GetValueProvider (playableInput, positionProviders, x => x.startPosition);
    }

    private QuaternionTweenProvider GetRotationProvider (ScriptPlayable<TransformDoTweenBehaviour> playableInput)
    {
        return GetValueProvider (playableInput, rotationProviders, x => Quaternion.Euler (x.startRotation));
    }

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

        for (int i = 0, inputCount = playable.GetInputCount (); i < inputCount; i++) {
            ScriptPlayable<TransformDoTweenBehaviour> playableInput =
                (ScriptPlayable<TransformDoTweenBehaviour>) playable.GetInput (i);

            float weight = playable.GetInputWeight (i);
            totalWeight += weight;

            VectorTweenProvider valueTweenProvider = GetPositionProvider (playableInput);
            blendedPosition += valueTweenProvider.Value * weight;

            QuaternionTweenProvider quaternionTweenProvider = GetRotationProvider (playableInput);
            Quaternion desiredRotation = quaternionTweenProvider.Value;
            desiredRotation = NormalizeQuaternion (desiredRotation);

            if (Quaternion.Dot (blendedRotation, desiredRotation) < 0f) {
                desiredRotation = ScaleQuaternion (desiredRotation, -1f);
            }

            desiredRotation = ScaleQuaternion (desiredRotation, weight);
            blendedRotation *= desiredRotation;
        }

        trackBinding.position = blendedPosition + (1f - totalWeight) * defaultPosition.Value;
        trackBinding.rotation = blendedRotation * ScaleQuaternion (defaultRotation.Value, 1f - totalWeight);
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        defaultPosition = null;
        defaultRotation = null;
    }

    static Quaternion ScaleQuaternion (Quaternion rotation, float multiplier)
    {
        return Quaternion.Euler (rotation.eulerAngles * multiplier);
    }

    static Quaternion NormalizeQuaternion (Quaternion rotation)
    {
        float magnitude = QuaternionMagnitude (rotation);

        if (magnitude > 0f)
            return ScaleQuaternion (rotation, 1f / magnitude);

        Debug.LogWarning ("Cannot normalize a quaternion with zero magnitude.");
        return Quaternion.identity;
    }

    static float QuaternionMagnitude (Quaternion rotation)
    {
        return Mathf.Sqrt ((Quaternion.Dot (rotation, rotation)));
    }
}