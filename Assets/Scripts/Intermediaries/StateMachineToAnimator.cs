using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StateMachineToAnimator : MonoBehaviour
{
    public State state;

    public void AnimationStartTrigger(int index)
    {
        state.AnimationStartTrigger(index);
    }

    public void AnimationAcitonTrigger(int index)
    {
        state.AnimationActionTrigger(index);
    }

    public void AnimationFinishTrigger(int index)
    {
        state.AnimationFinishTrigger(index);
    }
}
