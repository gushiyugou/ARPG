using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;


public interface IStateMachineOwner { }
public class StateMachine
{
    private IStateMachineOwner owner;

    private Dictionary<Type, StateBase> stateDictionary = new Dictionary<Type, StateBase>();

    public  Type CurrentStateType { get => currentState.GetType(); }
    public bool IsHasState { get => currentState != null; }
    private StateBase currentState;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="owner">传递的是宿主，该状态机实例的拥有者</param>
    public void Init(IStateMachineOwner owner)
    {
        this.owner = owner;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <typeparam name="T">要切换到的目标状态类型</typeparam>
    /// <param name="reCurrent">如果状态没变，是否需要刷新状态</param>
    /// <returns></returns>
    public bool ChangeState<T>(bool reCurrent = false) where T :StateBase,new()
    {
        //PlayerStateBase.GetPerviousState(currentState);
        //如果当前状态与要切换的状态的类型一致，且不需要刷新，则不切换
        if (IsHasState && CurrentStateType == typeof(T) && !reCurrent) return false;


        //退出当前状态、
        if (currentState != null)
        {
            currentState.Exit();
            MonoManager.Instance.RemoveUpdateListener(currentState.Update);
            MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
            MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        }

        currentState = GetState<T>();
        currentState.Enter();
        MonoManager.Instance.AddUpdateListener(currentState.Update);
        MonoManager.Instance.AddLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.AddFixedUpdateListener(currentState.FixedUpdate);

        //进入新状态
        return true;
    }


    private StateBase GetState<T>() where T : StateBase, new()
    {
        Type type = typeof(T);
        //如果状态缓存字典中存在T类型的状态，则返回这个状态
        if(!stateDictionary.TryGetValue(type, out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDictionary.Add(type, state);
        }
        return state;
    }

    
    
    /// <summary>
    /// 停止状态，释放资源
    /// </summary>
    public void Stop()
    {
        currentState.Exit();
        MonoManager.Instance.RemoveUpdateListener(currentState.Update);
        MonoManager.Instance.RemoveLateUpdateListener(currentState.LateUpdate);
        MonoManager.Instance.RemoveFixedUpdateListener(currentState.FixedUpdate);
        currentState = null;

        foreach(var item in stateDictionary.Values)
        {
            item.UnInit();
        }
        stateDictionary.Clear();
    }
}
