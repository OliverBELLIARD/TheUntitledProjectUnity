using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    enum State { CLEAR, RAIN, SANDSTORM, SNOW};

    public float temerature;
    public float humidity;

    [Header("Weather state")]
    State currentState = State.CLEAR;
    public bool rain;
    public bool sandStorm;
    public bool snow;

    [Header("References")]
    public DayNightController dnCtrl;
    public GameObject rainGenerator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WeatherSwitch();
        rainGenerator.SetActive(rain);
    }

    // Change current weather state to another random state
    public void RandomState()
    {
        temerature = Random.Range(-1, 1);
        humidity = Random.Range(-1, 1);
    }

    // Conditions for change
    void WeatherSwitch()
    {
        if(humidity <= 0f)
        {
            if (temerature <= 0f)
            {
                StateChange(State.CLEAR);
            }
            else
            {
                StateChange(State.SANDSTORM);
            }
        }
        else
        {
            if (temerature <= 0f)
            {
                StateChange(State.SNOW);
            }
            else
            {
                StateChange(State.RAIN);
            }
        }
    }

    // Change method and states actions
    void StateChange(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
        }

        if (currentState == State.CLEAR)
        {
            rain = false;
            sandStorm = false;
            snow = false;
        }
        else if (currentState == State.RAIN)
        {
            rain = true;
            sandStorm = false;
            snow = false;
        }
        else if (currentState == State.SANDSTORM)
        {
            rain = false;
            sandStorm = true;
            snow = false;
        }
        else if (currentState == State.SNOW)
        {
            rain = false;
            sandStorm = false;
            snow = true;
        }
    }
}
