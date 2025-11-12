using UnityEngine;

public class BossWalkState : BossStateBase
{
   
    public override void Enter()
    {
        boss.PlayAnimation("Walk");
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.walkSpeed;
    }

    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPos.transform.position);
        if (distance > boss.runRange)
        {
            boss.ChangeState(BossStateType.Run);
        }
        else
        {
            boss.navMeshAgent.SetDestination(boss.targetPos.transform.position);
        }


    }


    public override void Exit()
    {
        boss.navMeshAgent.enabled = true;
    }
}
