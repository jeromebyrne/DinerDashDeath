using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour {

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

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
