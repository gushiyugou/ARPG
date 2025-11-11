using UnityEngine;

public class BossIdleState : BossStateBase
{
   
    public override void Enter()
    {
        boss.PlayAnimation("Idle");
    }

    public override void Update()
    {
        boss.characterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
    }


    
}
