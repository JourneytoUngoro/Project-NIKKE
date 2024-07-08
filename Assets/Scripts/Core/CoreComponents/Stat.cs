using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public event Action OnCurrentValueZero;

    [field: SerializeField] public float maxValue { get; private set; }
    [field: SerializeField] public float currentValue { get; private set; }
    [field: SerializeField] public float recoveryTime { get; private set; } // recovery time of 0 means it does not recover automatically (ex. health)
    [field: SerializeField] public float recoveryValue { get; private set; }

    private Timer recoveryTimer;

    private bool onRecovery;

    public void Init()
    {
        recoveryTimer = new Timer(recoveryTime);
        recoveryTimer.timerAction += OnRecovery;
        currentValue = maxValue;
    }

    public void Recovery()
    {
        recoveryTimer.Tick();

        if (recoveryTime != 0 && onRecovery && currentValue < maxValue)
        {
            IncreaseCurrentValue(recoveryValue * Time.deltaTime);
        }
    }

    private void OnRecovery() => onRecovery = true;

    public void IncreaseCurrentValue(float amount)
    {
        currentValue += amount;
        currentValue = Mathf.Clamp(currentValue, 0.0f, maxValue);
    }

    public void DecreaseCurrentValue(float amount)
    {
        currentValue -= amount;
        currentValue = Mathf.Clamp(currentValue, 0.0f, maxValue);
        onRecovery = false;
        recoveryTimer.StartSingleUseTimer();
    }

    public void IncreaseMaxValue(float amount)
    {
        maxValue += amount;
        IncreaseCurrentValue(amount);
    }

    public void DecreaseMaxValue(float amount)
    {
        maxValue -= amount;
        Mathf.Clamp(currentValue, 0.0f, maxValue);
    }

    public float SliderValue() => currentValue / maxValue;
}
