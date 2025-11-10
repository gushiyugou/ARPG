
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModleBase:MonoBehaviour
{
    protected Animator _animator;
    public Animator _Animator { get { return _animator; } }
    protected Action<Vector3, Quaternion> rootMotionAction;
    //private CharacterController _controller;
    protected ISkillOwner skillOwner;
    [SerializeField] private WeaponController[] weapons;

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
