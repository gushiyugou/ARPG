
using UnityEngine;


public class BossController : CharacterBase
{
    public SkillHitData HitData { get; private set; }
    public ISkillOwner HitSource { get; private set; }


    private void Start()
    {
        Model.OnInit(this, enemyTagList);
        stateMachine = new StateMachine();
        model = GetComponentInChildren<BossModle>();
        stateMachine.Init(this);
        ChangeState(BossStateType.Idle);
    }

    


    //«–ªª◊¥Ã¨
    public void ChangeState(BossStateType needState,bool isCurrentState = false)
    {
        switch (needState)
        {
            case BossStateType.Idle:
                stateMachine.ChangeState<BossIdleState>();
                break;
            case BossStateType.Hurt:
                stateMachine.ChangeState<BossHurtState>(true);
                break;
        }
    }

    

    public override void Hurt(SkillHitData hitData, ISkillOwner hitSource)
    {
        HitData = hitData;
        HitSource = hitSource;
        Debug.Log("boss ‹…À");
        ChangeState(BossStateType.Hurt, true);
    }
}

