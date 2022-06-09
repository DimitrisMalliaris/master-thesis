using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance;
    [SerializeField] int InvokeCalls = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void InvokeInSeconds(Action action, float seconds)
    {
        StartCoroutine(InvokeAction(action, seconds));
        InvokeCalls++;
    }

    private IEnumerator InvokeAction(Action action, float duration)
    {
        yield return new WaitForSeconds(duration);
        action.Invoke();
        InvokeCalls--;
    }
}
