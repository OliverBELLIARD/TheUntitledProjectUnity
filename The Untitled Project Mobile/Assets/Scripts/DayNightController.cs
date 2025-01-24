using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightController : MonoBehaviour
{
    enum Cycles { DAY, SUNSET, NIGHT, SUNRISE };

    [Header("Days properties")]
    [Tooltip("Drag here the global light that will be used as a sun")]
    public Light2D sun;
    [Tooltip("Duration in minutes of a day cycle (24 in game hours). It also set the duration of an in game hour")]
    public float dayDuration = 1f;  // In real pysic minutes
    [Tooltip("In game hour of the start of the night")]
    public int endOfDayHour = 18;   // In game hours
    [Tooltip("In game hour of the end of the night")]
    public int endOfNightHour = 7;  // In game hours
    [Tooltip("Day color filter. The intensity is defined by the opacity of the color")]
    public Color dayColor;      // Day filter
    [Tooltip("Night color filter. The intensity is defined by the opacity of the color")]
    public Color nightColor;    // Night filter

    [Header("Transitions properties")]
    [Tooltip("Duration of the linear transition betwen cycles. Unit : in game hours")]
    public float fadeDuration = 1;  // In game hours
    [Tooltip("Color filter of the sunset. The intensity is defined by the opacity of the color")]
    public Color sunsetColor;
    [Tooltip("Color filter of the sunrise. The intensity is defined by the opacity of the color")]
    public Color sunriseColor;
    public float sunsetSpeed = 0.5f;
    [Tooltip("Duration of sunset. Unit : in game hours")]
    public float sunsetDuration = 1f;
    public float sunriseSpeed = 0.5f;
    [Tooltip("Duration of sunrise. Unit : in game hours")]
    public float sunriseDuration = 1f;

    [Space]
    [Header("Day state")]
    [Tooltip("Current time (in game hour)")]
    public int hour = 0;
    [Tooltip("Days passed during the execution (run of the script)")]
    public int nbDays = 0;

    [SerializeField]
    [Tooltip("Current cycle")]
    Cycles cycle = Cycles.DAY;

    int tick = 0;       // Counter tick
    float lenghtDay = 0;
    int fadeFactor;

    [Space]
    [Header("Outdoors Lights attributes")]
    [Tooltip("Drag here all the lights that have to be turned OFF during day and ON during night")]
    public GameObject[] outDoorsLights; // All outdoors lights that have to be tured off during day
    [Tooltip("In game hour to turn ON all the outdoors lights")]
    public int turnOnLightsHour = 20;
    [Tooltip("In game hour to turn OFF all the outdoors lights")]
    public int turnOffLightsHour = 7;

    [Space]
    [Header("Weather attributes")]
    public WeatherController weather;
    public Color stormDayColor;

    // Variables for counting
    float i = 0;
    float j = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        // 1s = 60 FixedUpdate cycles
        // 1min = 60s
        lenghtDay = 3600 * dayDuration;
        fadeFactor = (int)(fadeDuration * lenghtDay/2);

        // Start the script at midday
        tick = (int)(lenghtDay / 2);
        sun.color = dayColor;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tick++;
        DayCycle();
        OutdoorsLightsState();
    }

    // Change the Cycle duryng the current day
    void DayCycle()
    {
        hour = (int)(tick / (lenghtDay / 24));

        if (tick > lenghtDay)
        {
            tick = 0;
            nbDays++;
            weather.RandomState();
        }

        if (hour <= endOfNightHour || hour >= endOfDayHour)
        {
            CycleChange(Cycles.NIGHT);
        }
        else if (hour < endOfDayHour && hour >= (endOfDayHour - sunsetDuration))
        {
            CycleChange(Cycles.SUNSET);
        }
        else if (hour <= (endOfNightHour + sunriseDuration) && hour > endOfNightHour)
        {
            CycleChange(Cycles.SUNRISE);
        }
        else
        {
            CycleChange(Cycles.DAY);
        }
    }

    void CycleChange(Cycles newCycle)
    {
        // Change the cycle if the requested cycle is different
        if (cycle != newCycle)
        {
            cycle = newCycle;
            j = 0;
        }

        // Introduces different fade detween different Cycles
        if (cycle == Cycles.NIGHT)
        {
            sun.color = Color.Lerp(sun.color, nightColor, (j / fadeFactor) / dayDuration);
            j++;
        }
        else if (cycle == Cycles.SUNSET)
        {
            sun.color = Color.Lerp(sun.color, sunsetColor, (j / fadeFactor) / dayDuration);
            if (j < fadeFactor * sunsetSpeed)
                j++;
        }
        else if (cycle == Cycles.SUNRISE)
        {
            sun.color = Color.Lerp(sun.color, sunriseColor, j / (fadeFactor));
            if (j < fadeFactor * sunriseSpeed)
                j++;
        }
        else if (cycle == Cycles.DAY)
        {
            if (weather.rain)
                sun.color = Color.Lerp(sun.color, stormDayColor, j / fadeFactor);
            else
                sun.color = Color.Lerp(sun.color, dayColor, j / fadeFactor);
            j++;
        }
    }

    void OutdoorsLightsState()
    {
        if (hour <= turnOffLightsHour || hour >= turnOnLightsHour)
        {
            for (i = 0; i < outDoorsLights.Length; i++)
            {
                outDoorsLights[(int)i].SetActive(true);
            }
        }
        else
        {
            for (i = 0; i < outDoorsLights.Length; i++)
            {
                outDoorsLights[(int)i].SetActive(false);
            }
        }
    }
}
