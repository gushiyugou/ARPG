using Cinemachine;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class PlayerController : MonoBehaviour,IStateMachineOwner,ISkillOwner
{
    [Header("基础组件")]
    [SerializeField]private PlayerModle _playerModle;
    public PlayerModle _PlayerModle 
    { 
        get => _playerModle;  
    }

    [SerializeField]private CharacterController _characterController;
    public CharacterController _CharacterController{ get => _characterController; }

    private StateMachine _stateMachine;
    [SerializeField]private AudioSource _audioSource;

    //TODO 测试的配置信息，直接拖拽，后续会改
    //public SkillConfig _skillConfig;
    
    [Header("敌人Tag列表")]public List<string> enemyTagList;

    


    #region 配置信息
    public readonly float _gravity = -9.8f;
    [Space,Header("基础信息配置")]
    public float _rotationSpeed = 2f;
    public float walkToRunTransition = 1f;
    public float walkSpeed = 1f;
    public float RunSpeed = 1f;

    public float jumpStartSpeed = 1f;
    public float moveSpeedForJump = 2f;
    public float moveSpeedForAirDown = 2f;

    public float rotSpeed = 1f;

    /// <summary>
    /// 技能配置数组
    /// </summary>
    [Header("技能配置")]
    public SkillConfig[] standAttckCongig;
    public int currentHitWeapIndex;
    #endregion
    //拖尾组件
    [SerializeField,Header("拖尾插件")] private MeleeWeaponTrail weaponTrail;

    public CinemachineImpulseSource impulseSource;




    private void Awake()
    {
        //_playerModle = GetComponentWi<PlayerModle>();
        _PlayerModle.OnInit(this, enemyTagList);
        _stateMachine = new StateMachine();
        _stateMachine.Init(this);
        _characterController = GetComponent<CharacterController>();
        
    }

    private void Start()
    {
        ChangeState(PlayerStateType.Idle);
        
    }

    /// <summary>
    /// 状态切换
    /// </summary>
    /// <param name="needState">需要切换的状态类型</param>
    public void ChangeState(PlayerStateType needState)
    {
        switch (needState)
        {
            case PlayerStateType.Idle:
                _stateMachine.ChangeState<PlayerIdleState>();
                break;
            case PlayerStateType.Moevment:
                _stateMachine.ChangeState<PlayerMoveState>();
                break;
            case PlayerStateType.Jump:
                _stateMachine.ChangeState<PlayerJumpState>();
                break;
            case PlayerStateType.AirDown:
                _stateMachine.ChangeState<PlayerAirDownState>();
                break;
            case PlayerStateType.Sidestep:
                _stateMachine.ChangeState<PlayerSideStepState>();
                break;
            case PlayerStateType.SidestepReverse:
                _stateMachine.ChangeState<PlayerSidestepReverseState>();
                break;
            case PlayerStateType.AtkNormal1:
                _stateMachine.ChangeState<PlayerAtkNormal1State>();
                break;
        }
    }

    #region 技能相关
    private SkillConfig currentSkillConfig;
    private int currentHitIndex = 0;
    //技能发起攻击
    public void StartAttack(SkillConfig skillConfig)
    {
        
        currentSkillConfig = skillConfig;
        //表示是新技能的开始
        currentHitIndex = 0;

        //播放动画
        PlayAnimation(currentSkillConfig.AnimationName);
        //技能释放音效
        PlayAudio(currentSkillConfig.releaseData.skillAudio);
        //技能释放特效
        SpawnSkillObject(currentSkillConfig.releaseData.spawnObj);
        //击中检测

        //伤害传递
    }

    //攻击发起时，（实际的攻击动作，去掉了前摇和后摇的时间）
    public void StartSkillHit(int weaponIndex)
    {
        currentHitWeapIndex = weaponIndex;
        //技能释放音效
        PlayAudio(currentSkillConfig.attackData[currentHitIndex].attackAudio[currentHitWeapIndex]);
        //技能释放特效
        SpawnSkillObject(currentSkillConfig.attackData[currentHitIndex].skillObj);
        //特效释放
        weaponTrail.Emit = true;
    }

    //技能结束击中
    public void StopSkillHit(int weaponIndex)
    {
        currentHitIndex += 1;
        //如果用到了currentHitIndex，则currentHitIndex需要-1，也可以在攻击结束时，currentHitIndex+1
        weaponTrail.Emit = false;
    }

    //技能后摇的部分
    public void SkillCanSwitch()
    {

    }

    public void OnHit(IHurt target, Vector3 hitPosition)
    {
        //Debug.Log("角色控制：我攻击到了" + ((Component)target).gameObject.name);
        //OnHit在Stop之后执行，所以索引要减一
        Debug.Log(currentHitIndex);
        SkillAttackData skillData = currentSkillConfig.attackData[currentHitIndex];
        StartCoroutine(DoSkillEffect(skillData.hitEffect, hitPosition));

        //后处理,色差效果
        if(skillData.impulseValue != 0)
            ScreenImpulse(skillData.impulseValue);
        if (skillData.chromaticValue != 0)
            PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);
        //TODO:对IHurt传递伤害数据

    }

    //技能击中时的效果
    private IEnumerator DoSkillEffect(SkillHitEffectConfig hitEffetc, Vector3 pos)
    {
        if (hitEffetc == null) yield break;
        PlayAudio(hitEffetc.skillSpawnObj.spawnAudio);
        
        if (hitEffetc != null && hitEffetc.skillSpawnObj != null)
        {
            yield return new WaitForSeconds(hitEffetc.skillSpawnObj.Time);
            GameObject temp = Instantiate(hitEffetc.skillSpawnObj.prefab);
            temp.transform.position = pos + hitEffetc.skillSpawnObj.position;
            temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles = pos + hitEffetc.skillSpawnObj.rotation;
            PlayAudio(hitEffetc.hitAudioClip);
        }
    }

    private void SpawnSkillObject(SkillSpawnObj skillObj)
    {
        if (skillObj != null && skillObj.prefab != null)
        {
            StartCoroutine(DoSpawnObject(skillObj));
        }
    }

    private IEnumerator DoSpawnObject(SkillSpawnObj skillObj)
    {
        
        yield return new WaitForSeconds(skillObj.Time);
        GameObject skillPrefab = Instantiate(skillObj.prefab,null);
        skillPrefab.transform.position = _playerModle.transform.position + skillObj.position;
        //使用eulerAngles(欧拉角)来进行计算,移动和旋转都是模型层在做
        skillPrefab.transform.eulerAngles = _playerModle.transform.eulerAngles + skillObj.rotation;
    }

    #endregion


    public void ScreenImpulse(float force)
    {
        impulseSource.GenerateImpulse(force * 0.2f);
    }


    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名字</param>
    /// <param name="fixedTransitionDuration">过渡时间</param>
    public void PlayAnimation(string animationName,float fixedTransitionDuration = 0.25f)
    {
        _playerModle._Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
        
    }



    /// <summary>
    /// 立即播放动画
    /// </summary>
    /// <param name="animationName">动画名字</param>
    /// <param name="layer">动画的层级</param>
    /// <param name="num">开始播放的帧数</param>
    public void PlayAnimationImmediately(string animationName,int layer,int num)
    {
        _PlayerModle._Animator.Play(animationName, layer,num);
    }


    /// <summary>
    /// 检查当前是否正在播放指定动画
    /// </summary>
    public bool IsPlayingAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = _PlayerModle._Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }


    /// <summary>
    /// 强制动画过渡
    /// </summary>
    public void ForceAnimationTransition(string targetAnimation, float transitionDuration = 0.1f)
    {
        _PlayerModle._Animator.CrossFade(targetAnimation, transitionDuration);
    }


    /// <summary>
    /// 动画回调事件
    /// </summary>
    private void OnAnimatorMove()
    {
        // 应用根运动位移
        _characterController.Move(_playerModle._Animator.deltaPosition);
        transform.rotation *= _playerModle._Animator.deltaRotation;
        
    }

    public void OnFootStep()
    {
        //_audioSource.PlayOneShot(footStepAudioClips[Random.Range(0, footStepAudioClips.Length)]);
    }

    public void PlayAudio(AudioClip audioClip)
    {
        if(audioClip != null) 
            _audioSource.PlayOneShot(audioClip);
        
    }

    public void OnJumpLoopComplete()
    {
        if (_stateMachine.CurrentStateType == typeof(PlayerAirDownState))
        {
            // 动画完成后立即检查是否需要切换状态
            if (_CharacterController.isGrounded)
            {
                ChangeState(PlayerStateType.Idle);
            }
        }
    }

    
}
