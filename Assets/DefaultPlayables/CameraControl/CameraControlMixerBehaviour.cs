using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class CameraControlMixerBehaviour : PlayableBehaviour
{
    class FloatTweenProvider : ValueTweenProvider<float, CameraControlBehaviour>
    {
        protected override void RefreshTweener ()
        {
            CameraControlBehaviour input = GetInput ();
            tweener = DOTween
                .To (() => value, x => value = x, input.EndFieldOfView, Duration)
                .SetEase (input.Ease);
        }
    }
    
    private Dictionary<ScriptPlayable<CameraControlBehaviour>, FloatTweenProvider> FOWProviders =
        new Dictionary<ScriptPlayable<CameraControlBehaviour>, FloatTweenProvider> ();    
   
    private FloatTweenProvider GetFOWProvider (ScriptPlayable<CameraControlBehaviour> playableInput)
    {
        FloatTweenProvider FOWProvider;
        if (FOWProviders.TryGetValue (playableInput, out FOWProvider))
            return FOWProvider;

        FOWProvider = new FloatTweenProvider ();
        FOWProvider.Init (playableInput, x => x.StartFieldOfView);
        FOWProviders.Add (playableInput, FOWProvider);

        return FOWProvider;
    }

    private float? DefaultFOWValue;

    public override void ProcessFrame (Playable playable, FrameData info, object playerData)
    {
        Camera trackBinding = playerData as Camera;
        if (trackBinding == null)
            return;

        if (!DefaultFOWValue.HasValue)
            DefaultFOWValue = trackBinding.fieldOfView;        
        
        float totalWeight = 0f;
        float blendedFOW = 0f;
        for (int i = 0, inputCount = playable.GetInputCount (); i < inputCount; i++) {
            ScriptPlayable<CameraControlBehaviour> playableInput =
                (ScriptPlayable<CameraControlBehaviour>) playable.GetInput (i);

            float weight = playable.GetInputWeight (i);

            FloatTweenProvider FOWProvider = GetFOWProvider (playableInput);
            blendedFOW += FOWProvider.Value * weight;
            totalWeight += weight;
        }

        trackBinding.fieldOfView = blendedFOW + (1f - totalWeight) * DefaultFOWValue.Value;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        DefaultFOWValue = null;
    }
}