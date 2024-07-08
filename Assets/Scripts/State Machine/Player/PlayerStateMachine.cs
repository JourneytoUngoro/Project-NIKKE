using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public PlayerState currentState { get; private set; }
    public PlayerState prevState { get; private set; }
    public PlayerState nextState { get; private set; }

    public void Initialize(PlayerState staringState)
    {
        currentState = staringState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState nextState)
    {
        this.nextState = nextState;
        currentState.Exit();
        prevState = currentState;
        currentState = nextState;
        Debug.Log($"State changed to {currentState}");
        currentState.Enter();
    }
}
