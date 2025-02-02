using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    [field: SerializeField] public StatComponent experience { get; protected set; }
    [field: SerializeField] public StatComponent energy { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        energy.Init();

        // posture.OnCurrentValueMax += player.knockbackState.ShouldTransitToStunnedState;
    }

    protected override void Update()
    {
        base.Update();

        energy.Recovery();
    }
}
