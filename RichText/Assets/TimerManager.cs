/*
* ==============================================================================
*
* Description: Attach to a object in scene when you want to use functionalities of Timer.
* Version: 1.0
* Author: ChenXi
* Company: Kool Games
*
* Example:
* Timer timer = TimerManager.GetInstance().CreateTimer(durationInSeconds);
* timer.Tick += HandleFunction;
* timer.Start();
* ==============================================================================
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimerManager : MonoBehaviour
{

    private Dictionary<int, Timer> timers;
    private static TimerManager instance;
    private int timerid;
    private List<int> removeIds;

    void Awake()
    {
        instance = this;
    }

    public static TimerManager GetInstance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("TimerManager");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<TimerManager>();
        }
        return instance;
    }

    private TimerManager()
    {
        timers = new Dictionary<int, Timer>();
        removeIds = new List<int>();
        timerid = 0;
    }

    void Update()
    {
        foreach (var item in timers)
        {
            Timer t = (Timer)item.Value;
            if (t.IsStop())
            {
                removeIds.Add(t.timerId);
            }
            else if (t.IsStarted())
            {
                t.Update(Time.deltaTime);
            }
        }
        if (removeIds.Count > 0)
        {
            foreach (int i in removeIds)
            {
                timers[i] = null;
                timers.Remove(i);
            }
            removeIds.Clear();
        }
    }

    private void RemoveTimer(int timerId)
    {
        if (timers.ContainsKey(timerId))
        {
            timers.Remove(timerId);
        }
    }

    public Timer CreateTimer(float duration, TimerType type = TimerType.Once, int times = -1)
    {
        ++timerid;
        Timer t = new Timer(duration, timerid, type, times);
        timers.Add(timerid, t);
        return t;
    }

    public Timer GetTimer(int key)
    {
        if (timers.ContainsKey(key))
        {
            return (Timer)timers[key];
        }
        return null;
    }
}
