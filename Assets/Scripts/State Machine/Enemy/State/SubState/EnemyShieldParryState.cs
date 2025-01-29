using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShieldParryState : EnemyState
{
    public bool isShieldAvail { get; private set; }
    public bool isParryAvail { get; private set; }

    public Timer shieldCoolDownTimer;
    public Timer parryCoolDownTimer;

    public EnemyShieldParryState(Enemy enemy, string animBoolName) : base(enemy, animBoolName)
    {
        isShieldAvail = true;
        isParryAvail = true;
        shieldCoolDownTimer = new Timer(0.5f);
        shieldCoolDownTimer.timerAction += () => { isShieldAvail = true; };
        parryCoolDownTimer = new Timer(0.5f);
        parryCoolDownTimer.timerAction += () => { isParryAvail = true; };
    }
}
