using System.Collections;
using UnityEngine;

public class BossWalkState : BossStateBase
{
    private bool isVigilant;
   
    public override void Enter()
    {
        boss.PlayAnimation("Walk");
        boss.navMeshAgent.enabled = true;
        boss.navMeshAgent.speed = boss.walkSpeed;

        if (boss.anger)
            isVigilant = false;
        else
            isVigilant = Random.Range(0, 5) >= 3;

        if (isVigilant)
        {
            boss.navMeshAgent.updateRotation = true;
            boss.navMeshAgent.speed = boss.vigilantSpeed;
            stopVigilantCoroutine = MonoManager.Instance.StartCoroutine(StopVigilant());
        }
        else
            boss.navMeshAgent.speed = boss.walkSpeed;
    }



    Coroutine stopVigilantCoroutine;

    IEnumerator StopVigilant()
    {
        yield return new WaitForSeconds(Random.Range(0,boss.vigilantTime));
        isVigilant = false;
        boss.navMeshAgent.updateRotation = false;
        boss.navMeshAgent.speed = boss.walkSpeed;
        stopVigilantCoroutine=null;
        boss.PlayAnimation("Walk", false);
    }
    public override void Update()
    {
        float distance = Vector3.Distance(boss.transform.position, boss.target.transform.position);
        if (distance > boss.runRange)
        {
            boss.ChangeState(BossStateType.Run);
            return;
        }
        if (isVigilant)
        {
            Vector3 playerPos = boss.target.transform.position;
            boss.transform.LookAt(new Vector3(playerPos.x, boss.transform.position.y, playerPos.z));
            Vector3 targetPos = (boss.transform.position - playerPos).normalized * boss.vigilantRange + playerPos;
            Vector3 vigilantPos = new Vector3(targetPos.x + Random.Range(0,3), targetPos.y, targetPos.z);
            if (Vector3.Distance(vigilantPos, boss.transform.position) < 0.5f)
            {
                boss.PlayAnimation("Idle",false);
            }
            else
            {
                boss.PlayAnimation("Walk",false);
                boss.navMeshAgent.SetDestination(vigilantPos);
            }
        }
        else
        {
            if (distance <= boss.atkRange)
                boss.ChangeState(BossStateType.Attack);
            else
                boss.navMeshAgent.SetDestination(boss.target.transform.position);
        }
    }
    public override void Exit()
    {
        boss.navMeshAgent.enabled = true;
        if(stopVigilantCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(stopVigilantCoroutine);
            stopVigilantCoroutine = null;
        }
    }
}
