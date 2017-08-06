using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager {

    float currentTimeScale = 2.0f;

    static TimeManager timeManager = new TimeManager();

    public float GetCurrentTimescale()
    {
        return currentTimeScale;
    }

    public void SetCurrentTimescale(float value)
    {
        currentTimeScale = value;
    }

    public static TimeManager GetInstance()
    {
        return timeManager;
    }

    public float GetTimeDelta()
    {
        return Time.deltaTime * currentTimeScale;
    }
}
