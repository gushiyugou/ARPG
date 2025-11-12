using UnityEngine;

public class BossIdleState : BossStateBase
{
   
    public override void Enter()
    {
        boss.transform.LookAt(boss.transform.position);
        boss.PlayAnimation("Idle");
    }

    public override void Update()
    {
        boss.characterController.Move(new Vector3(0, boss._gravity * Time.deltaTime, 0));

        float distance = Vector3.Distance(boss.transform.position,boss.targetPos.transform.position);
        //if(distance < boss.atkRange)
        //{
        //    boss.ChangeState(BossStateType.Attack);
        //}
        if(distance > boss.runRange)
        {
            boss.ChangeState(BossStateType.Run);
        }
        else
        {
            boss.ChangeState(BossStateType.Walk);
        }

        //if (Input.GetMouseButtonDown(1) && boss.CanSwitchSkill)
        //{
        //    Debug.Log("进入切换逻辑");
        //    boss.ChangeState(BossStateType.Attack);
        //    return;
        //}
    }
}
