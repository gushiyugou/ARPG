using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    
    #region 配置信息
    
    [Space,Header("基础信息配置")]
    public float _rotationSpeed = 2f;
    public float walkToRunTransition = 1f;
    public float walkSpeed = 1f;
    public float RunSpeed = 1f;

    public float jumpStartSpeed = 1f;
    public float moveSpeedForJump = 2f;
    public float moveSpeedForAirDown = 2f;

    public float rotSpeed = 1f;
    public float attackRotSpeed = 1f;

    public Collider[] enemyCollider;
    public AudioClip[] atkEndAudioClip;

    public float waitCounterattackTime;
    public SkillConfig counterattackConfig;

    public BossController targetPos;
    [Header("技能信息")]
    
    #endregion

    public CinemachineImpulseSource impulseSource;

    public bool isDefence { get => currentState == PlayerStateType.Defence; }

    private void Start()
    {
        Init();
        (Model as PlayerModle).AddAtkEndAudio(PlayAtkEndAudio);
        Cursor.lockState = CursorLockMode.Locked;
        //_playerModle = GetComponentWi<PlayerModle>();
        attackDetector = Model.GetComponent<SectorAttackDetector>();
        ChangeState(PlayerStateType.Idle);
    }


    private void Update()
    {
        //玩家死亡时也会执行，存在不严谨性。
        UpdateSkillCdTime();
    }

    private PlayerStateType currentState;
    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="needState">需要切换的状态类型</param>
    public void ChangeState(PlayerStateType needState,bool reCurrent = false)
    {
        currentState = needState;
        switch (needState)
        {
            case PlayerStateType.Idle:
                stateMachine.ChangeState<PlayerIdleState>(reCurrent);
                break;
            case PlayerStateType.Moevment:
                stateMachine.ChangeState<PlayerMoveState>(reCurrent);
                break;
            case PlayerStateType.Jump:
                stateMachine.ChangeState<PlayerJumpState>(reCurrent);
                break;
            case PlayerStateType.AirDown:
                stateMachine.ChangeState<PlayerAirDownState>(reCurrent);
                break;
            case PlayerStateType.Sidestep:
                stateMachine.ChangeState<PlayerSideStepState>(reCurrent);
                break;
            case PlayerStateType.SidestepReverse:
                stateMachine.ChangeState<PlayerSidestepReverseState>(reCurrent);
                break;
            case PlayerStateType.Hurt:
                stateMachine.ChangeState<PlayerHurtState>(reCurrent);
                break;
            case PlayerStateType.AtkNormal1:
                stateMachine.ChangeState<PlayerAtkNormal1State>(reCurrent);
                break;
            case PlayerStateType.SkillAttack:
                stateMachine.ChangeState<PlayerSkillState>(reCurrent);
                break;
            case PlayerStateType.Defence:
                stateMachine.ChangeState<PlayerDefenceState>(reCurrent);
                break;
        }
    }


    public override void OnHit(IHurt target, Vector3 hitPosition)
    {
        //Debug.Log("角色控制：我攻击到了" + ((Component)target).gameObject.name);
        //OnHit在Stop之后执行，所以索引要减一
        SkillAttackData skillData = CurrentSkillConfig.attackData[currentHitIndex];
        PlayAudio(skillData.hitEffect.hitAudioClip);

        //对IHurt传递伤害数据
        //TODO:后续做特殊情况的处理
        if (target.Hurt(skillData.hitDatat, this))
        {
            StartCoroutine(DoSkillHitEffect(skillData.hitEffect.skillSpawnObj, hitPosition));
            //后处理,色差效果
            if (skillData.impulseValue != 0)
                ScreenImpulse(skillData.impulseValue);
            if (skillData.chromaticValue != 0)
                PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);

            DoFreezeFrameTime(skillData.FreezeFrameTime);

            DoFreezeGame(skillData.FreezeGameTime);
            Debug.Log("击中");
        }
        else
        {
            StartCoroutine(DoSkillHitEffect(skillData.hitEffect.failSkillSpawnObj, hitPosition));
        }

    }

    public override void AttackEffectCheck(SkillAttackData attackData)
    {
        Vector3 checkPos = Model.transform.position +
            Model.transform.TransformDirection(attackData.attackcheck.checkPos);

        Quaternion checkRot = Model.transform.rotation * Quaternion.Euler(attackData.attackcheck.checkRot);

        LayerMask enemyLayerMask = LayerMask.GetMask("Enemy");

        Collider[] hitColliders = new Collider[10];

        int numColliders = Physics.OverlapBoxNonAlloc(
            checkPos,
            attackData.attackcheck.halfExtents,
            hitColliders,
            checkRot,
            enemyLayerMask,
            QueryTriggerInteraction.UseGlobal
        );

        if (numColliders > 0)  // 修改为 > 0 而不是 != 0
        {
            // 找到第一个有效的碰撞体
            Collider validCollider = null;
            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i] != null)
                {
                    validCollider = hitColliders[i];
                    enemyList.Add(validCollider.gameObject.GetComponent<IHurt>());
                    break;
                }
            }

            if (validCollider != null)
            {
                Debug.Log("范围内有敌人");
                Debug.Log("敌人名字" + ((Component)enemyList[0]).gameObject.name);
                SkillAttackData skillData = CurrentSkillConfig.attackData[currentHitIndex];
                StartCoroutine(DoSkillHitEffect(skillData.skillObj, validCollider.ClosestPoint(weapon.position)));

                //后处理,色差效果
                if (skillData.impulseValue != 0)
                    ScreenImpulse(skillData.impulseValue);
                if (skillData.chromaticValue != 0)
                    PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);

                DoFreezeFrameTime(skillData.FreezeFrameTime);
                DoFreezeGame(skillData.FreezeGameTime);
            }
            else
            {
                Debug.LogWarning("检测到碰撞体但都为null");
                DrawDebugBox(checkPos, attackData.attackcheck.halfExtents, checkRot, Color.yellow);
            }
        }
        else
        {
            Debug.LogWarning("未检测到任何敌人");
            DrawDebugBox(checkPos, attackData.attackcheck.halfExtents, checkRot, Color.red);
        }
    }

    public void ScreenImpulse(float force)
    {
        impulseSource.GenerateImpulse(force * 0.2f);
    }


    public void OnJumpLoopComplete()
    {
        if (stateMachine.CurrentStateType == typeof(PlayerAirDownState))
        {
            // 动画完成后立即检查是否需要切换状态
            if (characterController.isGrounded)
            {
                ChangeState(PlayerStateType.Idle);
            }
        }
    }

    public override bool Hurt(SkillHitData hitData, ISkillOwner hitSource)
    {
        SetHurtData(hitData, hitSource);
        Debug.Log("玩家受伤");


        
        bool isDefence = currentState == PlayerStateType.Defence;
        if (isDefence && hitData.isBreak)
        {
            //破防，防御无效
            isDefence = false;
        }
        if (isDefence)
        {
            Transform enemyTransofrom = ((CharacterBase)hitSource).ModelTransform;
            Vector3 enemyToPlayerDir = (ModelTransform.position -  enemyTransofrom.position).normalized;
            float dot = Vector3.Dot(ModelTransform.forward, enemyToPlayerDir);
            if (dot > 0)
            {
                isDefence = false;
            }
            else
            {
                PlayerDefenceState defenceState = (PlayerDefenceState)stateMachine.CurrentState;
                defenceState.Hurt();
            }
        }
        if (!isDefence)
            ChangeState(PlayerStateType.Hurt, true);
        return !isDefence;
    }

    public void PlayAtkEndAudio(int index)
    {
        PlayAudio(atkEndAudioClip[index]);
    }


    /// <summary>
    /// 技能状态检测
    /// </summary>
    /// <returns></returns>
    public bool CheckAndEnterSkillState()
    {
        if (!CanSwitchSkill) return false;

        for(int i = 0; i< skillList.Count; i++)
        {
            if (skillList[i].currentTime ==0 && Input.GetKeyDown(skillList[i].skillKey))
            {
                ChangeState(PlayerStateType.SkillAttack, true);
                PlayerSkillState skillAttackState = (PlayerSkillState)stateMachine.CurrentState;
                skillAttackState.InitData(skillList[i].skillConfig);
                //技能进入cd状态
                skillList[i].currentTime = skillList[i].cdTime;
                return true;
            }
        }
       
        return false;
    }


    public void UpdateSkillCdTime()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            skillList[i].currentTime = Mathf.Clamp(skillList[i].currentTime - Time.deltaTime, 0, skillList[i].cdTime);
            skillList[i].skillMaskImg.fillAmount = skillList[i].currentTime/skillList[i].cdTime;
        }
    }
}