
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModelBase:MonoBehaviour
{
    protected Animator _animator;
    public Animator _Animator { get { return _animator; } }
    protected Action<Vector3, Quaternion> rootMotionAction;
    //private CharacterController _controller;
    protected ISkillOwner skillOwner;
    public WeaponController[] weapons;

    public void OnInit(ISkillOwner skillOwner, List<string> enemyTagList)
    {

        this.skillOwner = skillOwner;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(enemyTagList, skillOwner.OnHit);
        }
    }

    protected void Awake()
    {
        _animator = GetComponent<Animator>();
        //_controller = GetComponent<CharacterController>();
    }
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }

    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }




    #region 根运动
    protected void OnAnimatorMove()
    {
        this.rootMotionAction?.Invoke(_animator.deltaPosition, _animator.deltaRotation);
        //_controller.Move(_animator.deltaPosition);

    }
    #endregion



    #region 动画事件


    protected Action<string> runStopAction;
    protected void FootStep()
    {
        skillOwner.OnFootStep();
    }

    //private void JumpAudio()
    //{
    //    jumpAction?.Invoke();
    //}

    protected void StartSkillHit(int weaponIndex)
    {
        // 安全检查但不阻止执行
        if (weapons == null)
        {
            Debug.LogError("Weapons数组为null！");
            // 即使有问题也通知skillOwner，让它决定如何处理
            skillOwner.StartSkillHit(weaponIndex);
            return;
        }

        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            Debug.LogError($"武器索引越界: index={weaponIndex}, 数组长度={weapons.Length}");
            skillOwner.StartSkillHit(weaponIndex); // 仍然通知，但跳过武器操作
            return;
        }
        skillOwner.StartSkillHit(weaponIndex);
        weapons[weaponIndex].StartSkillHit();
    }

    protected void StopSkillHit(int weaponIndex)
    {

        skillOwner.StopSkillHit(weaponIndex);
        weapons[weaponIndex].StopSkillHit();
    }

    protected void SkillCanSwitch()
    {
        skillOwner.SkillCanSwitch();
    }


    #endregion
}
