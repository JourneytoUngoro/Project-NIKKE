using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachineToAnimator : MonoBehaviour
{
    public PlayerState state;

    public void AnimationStartTrigger()
    {
        state.AnimationStartTrigger();
    }

    public void AnimationAcitonTrigger()
    {
        state.AnimationActionTrigger();
    }

    public void AnimationFinishTrigger()
    {
        state.AnimationFinishTrigger();
    }
}
