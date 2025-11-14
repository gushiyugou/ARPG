using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BossIdleState : BossStateBase
{
   
    public override void Enter()
    {
        Vector3 pos = boss.transform.position;
        boss.transform.LookAt(new Vector3(pos.x, boss.transform.position.y, pos.z));
        //boss.transform.LookAt(boss.transform.position);
        boss.PlayAnimation("Idle");
    }

    public override void Update()
    {
        boss.characterController.Move(new Vector3(0, boss._gravity * Time.deltaTime, 0));

        float distance = Vector3.Distance(boss.transform.position,boss.target.transform.position);
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
