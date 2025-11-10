using UnityEngine;
public class BossStateBase : StateBase
{
    public BossController boss;
    public BossModle bossModle;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        boss = owner as BossController;
    }
}
