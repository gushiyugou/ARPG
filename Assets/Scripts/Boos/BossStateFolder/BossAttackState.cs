
using UnityEngine;


public class BossAttackState : BossStateBase
{
    //private float rotSpeed = 2f;
    private int currentSkillIndex;
    public int CurrentSkillIndex
    {
        get => currentSkillIndex;
        set
        {
            if (value >= boss.standAttckCongigs.Length)
                currentSkillIndex = 0;
            else 
                currentSkillIndex = value;
        }

    }

    public override void Enter()
    {
        //处理攻击方向
        currentSkillIndex = -1;
        boss.Model.SetRootMotionAction(OnRootMotion);
        StandAttck();
    }

    private void StandAttck()
    {
        
        //TODO：实现连续普攻
        CurrentSkillIndex += 1;
        boss.StartAttack(boss.standAttckCongigs[currentSkillIndex]);
        
        
    }
    public override void Update()
    {
        
        if (CheckAnimatorStateName(boss.standAttckCongigs[currentSkillIndex].AnimationName, out float animTime) && animTime >=0.9f)
        {
            //boss.Model._Animator.SetBool("IsIdle", true);
            boss.ChangeState(BossStateType.Idle);
            return;
        }

        if (Input.GetMouseButtonDown(1) && boss.CanSwitchSkill)
        {
            StandAttck();
            return;
        }
    }

    
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss._gravity * Time.deltaTime;
        boss.characterController.Move(deltaPosition);
    }

    public override void Exit()
    {
        Debug.Log($"BossAttackState退出时间: {Time.time}");
        boss.OnSkillOver();
    }
}
