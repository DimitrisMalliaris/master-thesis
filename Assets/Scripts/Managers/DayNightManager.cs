using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DayStatus
{
    Day,
    Night
}

public class DayNightManager : MonoBehaviour
{
    public const float MinutesInDay = 1440f / 10f;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float time = 0f;
    [Range(1f,30f)][SerializeField] float timeScale = 1f;
    [SerializeField] Light lightSource;
    public DayStatus Status;

    public static DayNightManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lightSource.intensity = animationCurve.Evaluate(time / MinutesInDay);
    }

    void Update()
    {
        // Increment timer
        time += timeScale * Time.deltaTime;

        // Restart timer
        if (time >= MinutesInDay)
            time = 0f;

        //
        float timeInDayFactor = time / MinutesInDay;

        // Set light intensity
        lightSource.intensity = animationCurve.Evaluate(timeInDayFactor);

        // Rotate light
        float xRot = 90f;

        if (timeInDayFactor <= 0.4f)
            xRot = Mathf.Lerp(90f, 185f, timeInDayFactor / 0.35f);
        else if (timeInDayFactor >= 0.7f)
            xRot = Mathf.Lerp(-5f, 90f, (timeInDayFactor - 0.65f) / 0.35f);

        lightSource.transform.rotation = Quaternion.Euler(xRot, 90, 0);

        // Set Day status
        if (lightSource.intensity < 0.1f)
            Status = DayStatus.Night;
        else
            Status = DayStatus.Day;
    }
}
