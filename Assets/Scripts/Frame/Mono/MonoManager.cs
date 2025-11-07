using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoManager : SingletonMono<MonoManager>
{
    private Action UpdateAction;
    private Action LateUpdateAction;
    private Action FixedUpdateAction;



    public void AddUpdateListener(Action action)
    {
        UpdateAction += action;
    }

    public void RemoveUpdateListener(Action action)
    {
        UpdateAction -= action;
    }

    public void AddLateUpdateListener(Action action)
    {
        LateUpdateAction += action;
    }

    public void RemoveLateUpdateListener(Action action)
    {
        LateUpdateAction -= action;
    }

    public void AddFixedUpdateListener(Action action)
    {
        FixedUpdateAction += action;
    }

    public void RemoveFixedUpdateListener(Action action)
    {
        FixedUpdateAction -= action;
    }


    private void Update()
    {
        UpdateAction?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateAction?.Invoke();
    }

    private void FixedUpdate()
    {
        FixedUpdateAction?.Invoke();
    }
}
