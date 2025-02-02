using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Stats
{
    [field: SerializeField] public StatComponent detection { get; protected set; }
    
    [SerializeField] private GameObject canvas;
    private Enemy enemy;
    private Timer canvasDisableTimer;

    protected override void Awake()
    {
        base.Awake();

        enemy = entity as Enemy;
        canvas = entity.GetComponentInChildren<Canvas>(true).gameObject;

        // posture.OnCurrentValueMax += enemy.knockbackState.ShouldTransitToStunnedState;
        posture.OnCurrentValueMax += () => { Debug.Log("ShouldTransitToStunnedState"); };
        canvasDisableTimer = new Timer(enemy.enemyData.canvasDisableTime);
        detection.OnCurrentValueMax += () => { enemy.sleepState.WakeUp(); };
        // health.OnCurrentValueZero += () => { enemy.isDead = true; };
    }

    protected override void Update()
    {
        base.Update();

        canvasDisableTimer.Tick();
    }

    public void EnableCanvas()
    {
        canvas.SetActive(true);
        canvasDisableTimer.StartSingleUseTimer();
    }

    public void DisableCanvas()
    {
        canvas.SetActive(false);
    }
}
