public class BossHurtState : BossStateBase
{
    public override void Enter()
    {
        boss.PlayAnimation("Hurt");
    }
}
