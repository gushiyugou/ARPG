
using UnityEngine;
using UnityEngine.AI;


public class BossController : CharacterBase
{
    public NavMeshAgent navMeshAgent;
    public float runRange = 10f;
    public float walkRange = 5f;
    public PlayerController targetPos;
    public float walkSpeed;
    public float runSpeed;


    private void Start()
    {
        Init();
        navMeshAgent = GetComponent<NavMeshAgent>();
        //stateMachine = new StateMachine();
        //model = GetComponentInChildren<BossModle>();
        stateMachine.Init(this);
        ChangeState(BossStateType.Idle);
        
    }

    


    //切换状态
    public void ChangeState(BossStateType needState,bool isCurrentState = false)
    {
        switch (needState)
        {
            case BossStateType.Idle:
                stateMachine.ChangeState<BossIdleState>(isCurrentState);
                break;
            case BossStateType.Walk:
                stateMachine.ChangeState<BossWalkState>(isCurrentState);
                break;
            case BossStateType.Run:
                stateMachine.ChangeState<BossRunState>(isCurrentState);
                break;
            case BossStateType.Hurt:
                stateMachine.ChangeState<BossHurtState>(isCurrentState);
                break;
            case BossStateType.Attack:
                stateMachine.ChangeState<BossAttackState>(isCurrentState);
                break;
        }
    }

    //攻击发起时，（实际的攻击动作，去掉了前摇和后摇的时间）
    public override void StartSkillHit(int weaponIndex)
    {
        //Debug.Log($"=== StartSkillHit被调用 ===");
        //Debug.Log($"时间: {Time.time}");
        //Debug.Log($"weaponIndex: {weaponIndex}");
        //Debug.Log($"weapons长度: {model.weapons?.Length}");
        // 添加状态检查
        //if (!CanSwitchSkill)  // 如果不在技能状态中，忽略攻击事件
        //{
        //    //Debug.LogWarning("接收到StartSkillHit但不在技能状态中，已忽略");
        //    return;
        //}
        currentHitWeapIndex = weaponIndex;
        //技能音效
        PlayAudio(CurrentSkillConfig.attackData[currentHitIndex].attackAudio);

        //技能的特效
        SpawnAttackEffect(CurrentSkillConfig.attackData[currentHitIndex].skillObj);

        //攻击检测
        //AttackEffectCheck(CurrentSkillConfig.attackData[currentHitIndex]);

        #region 扇形范围检测
        //扇形范围检测
        //List<GameObject> enemyList = attackDetector.DetectEnemiesInSector();
        //if (enemyList.Count > 0)
        //{
        //    IHurt enemy = enemyList[0].GetComponentInChildren<IHurt>();
        //    Collider enemyCollider = enemyList[0].GetComponent<Collider>();
        //    if (enemy != null)
        //    {
        //        Debug.Log("检测到敌人");
        //        OnHit(enemy, enemyCollider.ClosestPoint(weapon.position));
        //    }
        //}
        #endregion
        weaponTrail.Emit = true;
    }

    public override void Hurt(SkillHitData hitData, ISkillOwner hitSource)
    {
        base.Hurt(hitData, hitSource);
        Debug.Log("boss受伤");
        ChangeState(BossStateType.Hurt, true);
    }
}

