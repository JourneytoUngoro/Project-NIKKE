using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    protected Timer afterImageTimer;

    protected Vector2 workSpace;

    protected float epsilon = 0.001f;
    #endregion

    public float startTime {  get; protected set; }

    public virtual void AnimationStartTrigger(int index)
    {
        isAnimationStarted = true;
        isAnimationFinished = false;
    }

    public virtual void AnimationFinishTrigger(int index)
    {
        isAnimationFinished = true;
    }

    public virtual void AnimationActionTrigger(int index)
    {
        isAnimationActionTriggered = true;
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
