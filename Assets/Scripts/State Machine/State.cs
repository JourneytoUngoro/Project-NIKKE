using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    #region State Variables
    protected bool onStateExit;
    protected bool isAnimationStarted;
    protected bool isAnimationActionTriggered;
    protected bool isAnimationFinished;
    #endregion

    #region Physics Variables
    protected Vector2 currentPosition;
    protected Vector2 currentVelocity;

    protected int facingDirection;
    #endregion

    #region Other Variables
    protected Vector2 workSpace;

    protected float epsilon = 0.001f;
    #endregion

    public float startTime {  get; protected set; }

    public virtual void AnimationStartTrigger()
    {

    }

    public virtual void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }

    public virtual void AnimationActionTrigger()
    {

    }

    public virtual void DoChecks()
    {

    }

    public virtual void Enter()
    {
        startTime = Time.time;
        onStateExit = false;
        isAnimationStarted = false;
        isAnimationActionTriggered = false;
        isAnimationFinished = false;
    }
}
