using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : CoreComponent
{
    [field: SerializeField] public Stat health { get; protected set; }
    [field: SerializeField] public Stat posture { get; protected set; }

    protected override void Awake()
    {
        base.Awake();

        health.Init();
        posture.Init();
    }

    protected virtual void Update()
    {
        health.Recovery();
        posture.Recovery();
    }
}
