using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : CoreComponent
{
    [field: SerializeField] public Stat health { get; private set; }
    [field: SerializeField] public Stat posture { get; private set; }
    [field: SerializeField] public Stat energy { get; private set; }

    private void Awake()
    {
        health.Init();
        posture.Init();
        energy.Init();
    }

    private void Update()
    {
        /*if (!Manager.instance.gameManager.isPaused)
        {
            health.Recovery();
            poise.Recovery();
            energy.Recovery();
        }*/
        health.Recovery();
        posture.Recovery();
        energy.Recovery();
    }
}
