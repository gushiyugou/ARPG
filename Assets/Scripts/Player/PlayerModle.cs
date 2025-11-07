using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModle : MonoBehaviour
{
    private Animator _animator;
    public Animator _Animator { get { return _animator; } }
    private Action<Vector3, Quaternion> rootMotionAction;
    //private CharacterController _controller;
    private ISkillOwner skillOwner;
    [SerializeField]private WeaponController[] weapons;

    public void OnInit(ISkillOwner skillOwner, List<string> enemyTagList)
    {
        
        this.skillOwner = skillOwner;
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Init(enemyTagList, skillOwner.OnHit);
        }
    }

    private void Awake()
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
    private void OnAnimatorMove()
    {
        this.rootMotionAction?.Invoke(_animator.deltaPosition,_animator.deltaRotation);
        //_controller.Move(_animator.deltaPosition);

    }
    #endregion



    #region 动画事件

    
    private Action<string> runStopAction;
    private void FootStep()
    {
        skillOwner.OnFootStep();
    }

    //private void JumpAudio()
    //{
    //    jumpAction?.Invoke();
    //}

    private void StartSkillHit(int weaponIndex)
    {
        skillOwner.StartSkillHit(weaponIndex);
        weapons[weaponIndex].StartSkillHit();
    }

    private void StopSkillHit(int weaponIndex)
    {
        skillOwner.StopSkillHit(weaponIndex);
        weapons[weaponIndex].StopSkillHit();
    }

    private void SkillCanSwitch()
    {
        skillOwner.SkillCanSwitch();
    }


    #endregion
}
