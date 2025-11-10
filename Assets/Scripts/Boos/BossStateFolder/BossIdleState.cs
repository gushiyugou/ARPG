using UnityEngine;

public class BossIdleState : BossStateBase
{
    public override void Enter()
    {
        boss.PlayAnimation("Idle");
    }
}
