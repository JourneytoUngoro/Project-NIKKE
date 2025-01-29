using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }
    public EnemyState prevState { get; private set; }

    public void Initialize(EnemyState staringState)
    {
        currentState = staringState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState nextState)
    {
        currentState.Exit();
        prevState = currentState;
        currentState = nextState;
        Debug.Log($"State changed from {prevState} to {currentState}");
        currentState.Enter();
    }
}
