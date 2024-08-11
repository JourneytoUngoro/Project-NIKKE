using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [SerializeField] private GameObject canvas;

    protected override void Awake()
    {
        base.Awake();

        // posture.OnCurrentValueMax += enemy.knockbackState.ShouldTransitToStunnedState;
        posture.OnCurrentValueMax += () => { Debug.Log("ShouldTransitToStunnedState"); };
        // health.OnCurrentValueZero += () => { enemy.isDead = true; };
    }

    public void EnableCanvas()
    {
        canvas.SetActive(true);
    }

    public void DisableCanvas()
    {
        canvas.SetActive(false);
    }
}
