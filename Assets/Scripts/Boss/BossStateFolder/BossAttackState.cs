
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


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
    private SkillConfig currentSkillConfig;
    private float currentAttackTime = 0;
    public override void Enter()
    {
        //处理攻击方向
        currentSkillIndex = -1;
        currentAttackTime = 0;
        boss.Model.SetRootMotionAction(OnRootMotion);
        StandAttck();
    }

    private void StandAttck()
    {

        //TODO：实现连续普攻
        Vector3 pos = boss.target.transform.position;
        boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
        CurrentSkillIndex += 1;
        
        boss.StartAttack(boss.standAttckCongigs[currentSkillIndex]);
        currentSkillConfig = boss.standAttckCongigs[currentSkillIndex];




    }
    public override void Update()
    {
        currentAttackTime += Time.deltaTime;
        if (boss.CanSwitchSkill)
        {
            float distance = Vector3.Distance(boss.transform.position, boss.target.transform.position);
            if (distance <= boss.atkRange && currentAttackTime < boss.vigilantTime)
            {
                //是防御状态，则优先判断是否可见释放技能
                if (boss.target.isDefence)
                {
                    for (int i = 0; i < currentSkillIndex; i++)
                    {
                        if (boss.skillList[i].currentTime == 0 && boss.skillList[i].skillConfig.attackData.Length > 0)
                        {
                            if (boss.skillList[i].skillConfig.attackData[0].hitDatat.isBreak)
                            {
                                StartSkill(i);
                                return;
                            }
                        }
                    }
                }
                for (int i = 0; i < currentSkillIndex; i++)
                {
                    if (boss.skillList[i].currentTime == 0)
                    {
                        StartSkill(i);
                        return;
                    }
                }
                StandAttck();
            }
            else
            {
                boss.ChangeState(BossStateType.Walk);
            }
        }
        else if (currentSkillConfig != null && CheckAnimatorStateName( currentSkillConfig.AnimationName, out float animTime) && animTime >= 0.9f)
        {
            //_player.PlayAnimation($"Normal0{currentSkillIndex}_End");
            boss.ChangeState(BossStateType.Walk);
            return;
        }
    }

    
    private void StartSkill(int index)
    {
        Vector3 pos = boss.target.transform.position;
        boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
        boss.StartSkill(index);
        currentSkillConfig = boss.skillList[index].skillConfig;
    }
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = boss._gravity * Time.deltaTime;
        boss.characterController.Move(deltaPosition);
    }

    public override void Exit()
    {
        boss.Model.ClearRootMotionAction();
        Debug.Log($"BossAttackState退出时间: {Time.time}");
        boss.OnSkillOver();
        currentSkillConfig = null;
    }
}
