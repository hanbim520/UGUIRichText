/*
* ==============================================================================
*
* Description: Timer can't be used alone. Use TimerManager please.
* Version: 1.0
* Author: ChenXi
* Company: Kool Games
*
* ==============================================================================
*/
using UnityEngine;

public enum TimerType
{
    Once, Loop
}

public class Timer
{
    public delegate void TimerHandler();
    public event TimerHandler Tick;
    public event TimerHandler OnComplete;
    public int timerId;

    private float countDown;
    private float duration;
    private TimerType type;
    private int times;

    private enum Phase
    {
        NeverStart, Started, Stoped, Paused
    }
    private Phase state;

    public Timer(float duration, int timerId, TimerType type = TimerType.Once, int times = -1)
    {
        countDown = 0.0f;
        this.duration = duration;
        this.timerId = timerId;
        this.type = type;
        state = Phase.NeverStart;
        this.times = times;
    }

    public void Update(float deltaTime)
    {
        if (state == Phase.Started)
        {
            countDown += deltaTime;

            if (countDown >= duration)
            {
                if (Tick != null)
                {
                    Tick();
                }
                if (type == TimerType.Once)
                {
                    Stop();
                }
                else if (type == TimerType.Loop)
                {
                    if (times <= 0)
                    {
                        Start();
                    }
                    else if (times > 0)
                    {
                        --times;
                        if (times <= 0)
                        {
                            Stop();
                        }
                        else
                        {
                            Start();
                        }
                    }
                }
            }
        }
    }

    public void Start()
    {
        state = Phase.Started;
        countDown = 0.0f;
    }

    public void Stop()
    {
        state = Phase.Stoped;
        countDown = 0.0f;
        if (OnComplete != null)
        {
            OnComplete();
        }
    }

    public void Resume()
    {
        state = Phase.Started;
    }

    public void Pause()
    {
        state = Phase.Paused;
    }

    public bool IsPause()
    {
        return state == Phase.Paused;
    }

    public bool IsStop()
    {
        return state == Phase.Stoped;
    }

    public bool IsStarted()
    {
        return state == Phase.Started;
    }

    public void ResetDuration(float duration)
    {
        this.duration = duration;
    }

    public void Restart()
    {
        Start();
    }
}