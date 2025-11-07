using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 所有状态的基类
/// </summary>
public abstract class StateBase 
{
   /// <summary>
   /// 初始化
   /// 只在状态第一次创建时调佣
   /// </summary>
   /// <param name="owner">状态的宿主</param>
   /// <param name="stateType">状态的标识</param>
    public virtual void Init(IStateMachineOwner owner) { }

    /// <summary>
    /// 反初始化
    /// 销毁时释放一些资源
    /// </summary>
    public virtual void UnInit() { }

    /// <summary>
    /// 状态每次进入时都会执行一次
    /// </summary>
    public virtual void Enter(){}
    /// <summary>
    /// 状态退出时执行一次
    /// </summary>
    public virtual void Exit() { }
    /// <summary>
    /// 状态的不同周期函数，函数意义和继承Mono的脚本是一样的，只需要在对应的
    /// 周期函数中执行宿主的状态的对应周期函数即可。
    /// </summary>
    public virtual void Update() { }

    public virtual void LateUpdate() { }

    public virtual void FixedUpdate() { }

}
