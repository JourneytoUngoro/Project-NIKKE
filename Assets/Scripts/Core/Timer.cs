using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public event Action timerAction;

    private float duration;
    private float startTime;
    private float timeOffset;

    private bool timerActive;
    private bool isSingleUse;
    private bool resetStartTime;
    private bool isAdjustTimeSingleUse;

    private int maxMultiUseAmount;
    private int currentMultiUseAmount;

    public Timer(float duration)
    {
        this.duration = duration;
        timerActive = false;
        resetStartTime = true;
    }

    public void Tick()
    {
        if (timerActive)
        {
            if (Time.time + timeOffset > startTime + duration)
            {
                timerAction?.Invoke();

                if (isSingleUse)
                {
                    StopTimer();

                    if (isAdjustTimeSingleUse)
                    {
                        timeOffset = 0.0f;
                    }
                }
                else
                {
                    startTime = Time.time;

                    if (maxMultiUseAmount != 0)
                    {
                        if (currentMultiUseAmount < maxMultiUseAmount)
                        {
                            currentMultiUseAmount += 1;
                        }
                        else
                        {
                            StopTimer();
                        }
                    }
                }
            }
        }
    }

    public void Tick(bool condition)
    {
        if (condition)
        {
            if (timerActive)
            {
                if (Time.time + timeOffset > startTime + duration)
                {
                    timerAction?.Invoke();

                    if (isSingleUse)
                    {
                        StopTimer();

                        if (isAdjustTimeSingleUse)
                        {
                            timeOffset = 0.0f;
                        }
                    }
                    else
                    {
                        startTime = Time.time;

                        if (maxMultiUseAmount != 0)
                        {
                            if (currentMultiUseAmount < maxMultiUseAmount)
                            {
                                currentMultiUseAmount += 1;
                            }
                            else
                            {
                                StopTimer();
                            }
                        }
                    }
                }
            }
        }
    }

    public void TickResetTime(bool condition)
    {
        if (condition)
        {
            if (resetStartTime)
            {
                startTime = Time.time;
                resetStartTime = false;
            }

            if (timerActive)
            {
                if (Time.time + timeOffset > startTime + duration)
                {
                    timerAction?.Invoke();
                    
                    if (isSingleUse)
                    {
                        StopTimer();

                        if (isAdjustTimeSingleUse)
                        {
                            timeOffset = 0.0f;
                        }
                    }
                    else
                    {
                        startTime = Time.time;

                        if (maxMultiUseAmount != 0)
                        {
                            if (currentMultiUseAmount < maxMultiUseAmount)
                            {
                                currentMultiUseAmount += 1;
                            }
                            else
                            {
                                StopTimer();
                            }
                        }
                    }
                }
            }
        }
    }

    public void StartSingleUseTimer()
    {
        timerActive = true;
        isSingleUse = true;
        startTime = Time.time;
    }

    public void StartMultiUseTimer()
    {
        timerActive = true;
        isSingleUse = false;
        startTime = Time.time;
    }

    public void StartMultiUseTimer(int maxMultiUseAmount)
    {
        timerActive = true;
        isSingleUse = false;
        startTime = Time.time;
        currentMultiUseAmount = 0;
        this.maxMultiUseAmount = maxMultiUseAmount;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void AdjustTimeFlow(float adjustTimeAmount, bool isAdjustTimeSingleUse = true)
    {
        timeOffset = adjustTimeAmount;
        this.isAdjustTimeSingleUse = isAdjustTimeSingleUse;
    }
}
