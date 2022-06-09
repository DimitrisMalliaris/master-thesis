using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum SimMode
{
    Normal,
    Debug
}

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;

    [SerializeField] public SimMode Mode = SimMode.Normal;
    private readonly int DefaultTimeScale = 1;
    [SerializeField][Range(1,8)] int TimeScale = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Time.timeScale = DefaultTimeScale * TimeScale;
    }
}
