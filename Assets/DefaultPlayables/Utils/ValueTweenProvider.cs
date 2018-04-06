using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public abstract class ValueTweenProvider<TValue, TBehaviour>
    where TValue : struct
    where TBehaviour : class, IPlayableBehaviour, new ()
{
    private ScriptPlayable<TBehaviour> playableInput;
    protected Tweener tweener;
    protected TValue value;

    public void Init (
        ScriptPlayable<TBehaviour> playableInput,
        Func<TBehaviour, TValue> initialValueObtainer)
    {
        this.playableInput = playableInput;
        value = initialValueObtainer (playableInput.GetBehaviour ());
        RefreshTweener ();
    }

    protected abstract void RefreshTweener ();

    private bool ShouldRefreshTweener ()
    {
        return !Mathf.Approximately (tweener.Duration (), Duration);
    }

    protected TBehaviour GetInput ()
    {
        return playableInput.GetBehaviour ();
    }

    protected float Duration => (float) playableInput.GetDuration ();

    private float Time => (float) playableInput.GetTime ();

    public TValue Value {
        get {
            if (ShouldRefreshTweener ())
                RefreshTweener ();

            tweener.Goto (Time);
            return value;
        }
    }
}