using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class CameraMixerBehaviour : PlayableBehaviour
{
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
            ScriptPlayable<CameraDoTweenBehaviour> playableInput =
                (ScriptPlayable<CameraDoTweenBehaviour>) playable.GetInput (i);

            float weight = playable.GetInputWeight (i);
            CameraDoTweenBehaviour behaviour = playableInput.GetBehaviour ();

            blendedFOW += behaviour.Evaluate (playableInput.GetTime (), Durations[i]).FOW * weight;
            totalWeight += weight;
        }

        trackBinding.fieldOfView = blendedFOW + (1f - totalWeight) * DefaultFOWValue.Value;
    }

    public override void OnPlayableDestroy (Playable playable)
    {
        DefaultFOWValue = null;
    }

    public double[] Durations;
}