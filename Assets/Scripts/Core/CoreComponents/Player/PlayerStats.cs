using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    [field: SerializeField] public Stat energy { get; private set; }

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
