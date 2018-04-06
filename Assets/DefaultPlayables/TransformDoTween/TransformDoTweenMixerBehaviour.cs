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

    private bool TryCast<TBehaviour> (Playable playableInput, out ScriptPlayable<TBehaviour> scriptPlayable) where TBehaviour : class, IPlayableBehaviour, new()
    {
        try {
            scriptPlayable = (ScriptPlayable<TBehaviour>) playableInput;
            return true;
        } catch (InvalidCastException) {
            scriptPlayable = default (ScriptPlayable<TBehaviour>);
            return false;
        }
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
            Vector3 position = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            Playable playableInput = playable.GetInput (i);
            float weight = playable.GetInputWeight (i);
            totalWeight += weight;

            do {
                ScriptPlayable<TransformDoTweenBehaviour> doTweenInput;
                if (TryCast (playableInput, out doTweenInput)) {
                    position = GetPositionProvider (doTweenInput).Value;
                    rotation = GetRotationProvider (doTweenInput).Value;
                    
                    break;
                }

                ScriptPlayable<TransformControlBehaviour> transformControlInput;
                if (TryCast (playableInput, out transformControlInput)) {
                    TransformControlBehaviour behaviour = transformControlInput.GetBehaviour ();
                    position = behaviour.Position;
                    rotation = Quaternion.Euler (behaviour.Rotation);
                    
                    break;
                }
            } while (false);

            blendedPosition += position * weight;
            blendedRotation *= ScaleQuaternion (rotation, weight);
        }

        Debug.Log("position " + blendedPosition  + " " + totalWeight);
        Debug.Log("rotation " + blendedRotation.eulerAngles + " " + totalWeight);
        
        
        trackBinding.position = blendedPosition + (1f - totalWeight) * defaultPosition.Value;
        trackBinding.rotation = blendedRotation * ScaleQuaternion (defaultRotation.Value, 1f - totalWeight);
        
        Debug.Log("trackBinding.position " + trackBinding.position);
        Debug.Log("trackBinding.rotation " + trackBinding.rotation);
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
}