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

    #endregion

    public CinemachineImpulseSource impulseSource;

    private void Start()
    {
        Init();
        (Model as PlayerModle).AddAtkEndAudio(PlayAtkEndAudio);
        Cursor.lockState = CursorLockMode.Locked;
        //_playerModle = GetComponentWi<PlayerModle>();
        attackDetector = Model.GetComponent<SectorAttackDetector>();
        ChangeState(PlayerStateType.Idle);
    }

    

    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="needState">需要切换的状态类型</param>
    public void ChangeState(PlayerStateType needState,bool reCurrent = false)
    {
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
        }
    }


    public override void OnHit(IHurt target, Vector3 hitPosition)
    {
        //Debug.Log("角色控制：我攻击到了" + ((Component)target).gameObject.name);
        //OnHit在Stop之后执行，所以索引要减一
        SkillAttackData skillData = CurrentSkillConfig.attackData[currentHitIndex];
        StartCoroutine(DoSkillHitEffect(skillData.hitEffect, hitPosition));
        //后处理,色差效果
        if (skillData.impulseValue != 0)
            ScreenImpulse(skillData.impulseValue);
        if (skillData.chromaticValue != 0)
            PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);

        DoFreezeFrameTime(skillData.FreezeFrameTime);

        DoFreezeGame(skillData.FreezeGameTime);
        Debug.Log("击中");

        //对IHurt传递伤害数据
        //TODO:后续做特殊情况的处理
        target.Hurt(CurrentSkillConfig.attackData[currentHitIndex].hitDatat, this);

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
                StartCoroutine(DoSkillHitEffect(skillData.hitEffect, validCollider.ClosestPoint(weapon.position)));

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

    public override void Hurt(SkillHitData hitData, ISkillOwner hitSource)
    {
        base.Hurt(hitData, hitSource);
        Debug.Log("玩家受伤");
        ChangeState(PlayerStateType.Hurt, true);
    }

    public void PlayAtkEndAudio(int index)
    {
        PlayAudio(atkEndAudioClip[index]);
    }
}