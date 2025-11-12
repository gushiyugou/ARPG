using UnityEngine;

public class BossRunState : BossStateBase
{

    public override void Enter()
    {
        boss.PlayAnimation("Run");
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.runSpeed;
    }

    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.targetPos.transform.position);
        if (distance <= boss.runRange)
        {
            boss.ChangeState(BossStateType.Walk);
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
