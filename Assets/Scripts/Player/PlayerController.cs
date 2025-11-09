using Cinemachine;
using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.GraphView;

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

    public Collider[] enemyCollider;


    [Header("扇形攻击检测")]
    private SectorAttackDetector attackDetector;
    [SerializeField] private float baseAttackDamage = 10f;
    public Transform weapon;

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
        attackDetector = _PlayerModle.GetComponent<SectorAttackDetector>();
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
    


    /// <summary>
    /// 技能的攻击
    /// </summary>
    /// <param name="skillConfig"></param>
    public void StartAttack(SkillConfig skillConfig)
    {
        

        currentSkillConfig = skillConfig;
        //表示是新技能的开始
        currentHitIndex = 0;
        //播放动画
        PlayAnimation(currentSkillConfig.AnimationName);
        //技能释放时角色音效
        PlayAudio(currentSkillConfig.releaseData.skillAudio);
        //技能释放时角色的特效
        SpawnSkill(currentSkillConfig.releaseData.effectObj);
        //击中检测

        //伤害传递
    }



    //攻击发起时，（实际的攻击动作，去掉了前摇和后摇的时间）
    public void StartSkillHit(int weaponIndex)
    {
        currentHitWeapIndex = weaponIndex;
        //技能音效
        PlayAudio(currentSkillConfig.attackData.attackAudio);
        //技能的特效
        SpawnAttackEffect(currentSkillConfig.attackData.skillObj);

        //攻击检测
        AttackEffectCheck(currentSkillConfig.attackData);

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
        SkillAttackData skillData = currentSkillConfig.attackData;
        StartCoroutine(DoSkillHitEffect(skillData.hitEffect, hitPosition));
        //后处理,色差效果
        if(skillData.impulseValue != 0)
            ScreenImpulse(skillData.impulseValue);
        if (skillData.chromaticValue != 0)
            PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);

        DoFreezeFrameTime(skillData.FreezeFrameTime);

        DoFreezeGame(skillData.FreezeGameTime);
        Debug.Log("击中");
        //TODO:对IHurt传递伤害数据

    }


    /// <summary>
    /// 攻击检测
    /// </summary>
    /// <param name="skillConfig"></param>
    public void AttackEffectCheck(SkillAttackData attackData)
    {
        Vector3 checkPos = _PlayerModle.transform.position +
                          _PlayerModle.transform.forward * attackData.attackcheck.checkPos.z +
                          _PlayerModle.transform.up * attackData.attackcheck.checkPos.y +
                          _PlayerModle.transform.right * attackData.attackcheck.checkPos.x;

        Quaternion checkRot = _PlayerModle.transform.rotation * Quaternion.Euler(attackData.attackcheck.checkRot);

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
        if(numColliders != 0)
        {
            //Vector3 hitPosition = hitColliders[0].gameObject.transform.position;
            Debug.Log("范围内有敌人");
            SkillAttackData skillData = currentSkillConfig.attackData;
            StartCoroutine(DoSkillHitEffect(skillData.hitEffect, hitColliders[0].ClosestPoint(weapon.position)));
            //后处理,色差效果
            if (skillData.impulseValue != 0)
                ScreenImpulse(skillData.impulseValue);
            if (skillData.chromaticValue != 0)
                PostProcessingManager.Instance.ChromaticAberrationEF(skillData.chromaticValue);

            DoFreezeFrameTime(skillData.FreezeFrameTime);

            DoFreezeGame(skillData.FreezeGameTime);
        }
        //else
        //{
        //    Debug.LogWarning(" 未检测到任何敌人");

        //    // 绘制调试图形
        //    DrawDebugBox(checkPos, attackData.attackcheck.halfExtents, checkRot, Color.red);
        //}
    }




    /// <summary>
    /// 卡肉效果
    /// </summary>
    /// <param name="force"></param>
    public void DoFreezeFrameTime(float time)
    {
        StartCoroutine(StartFreezeFrameTime(time));
    }
    public IEnumerator StartFreezeFrameTime(float time)
    {
        _playerModle._Animator.speed = 0; 
        yield return new WaitForSeconds(time);
        _playerModle._Animator.speed = 1;
    }



    /// <summary>
    /// 游戏停止
    /// </summary>
    /// <param name="force"></param>
    public void DoFreezeGame(float time)
    {
        StartCoroutine(StartFreezeGameTime(time));
    }
    public IEnumerator StartFreezeGameTime(float time)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(time);
        Time.timeScale = 1;
    }




    //技能击中时的效果
    private IEnumerator DoSkillHitEffect(SkillHitEffectConfig hitEffetc, Vector3 pos)
    {
        if (hitEffetc == null) yield break;
        PlayAudio(hitEffetc.skillSpawnObj.spawnAudio);
        
        if (hitEffetc != null && hitEffetc.skillSpawnObj != null)
        {
            yield return new WaitForSeconds(hitEffetc.skillSpawnObj.Time);
            //直接生成预制体
            //GameObject temp = Instantiate(hitEffetc.skillSpawnObj.prefab);
            //使用对象池生成预制体
            GameObject temp = PoolManager.Instance.GetPoolObject(hitEffetc.skillSpawnObj.prefab.prefabName,
                hitEffetc.skillSpawnObj.prefab.folderName);
            Debug.Log("触发了");

            temp.transform.position = pos + hitEffetc.skillSpawnObj.position;
            //temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles = pos + hitEffetc.skillSpawnObj.rotation;
            PlayAudio(hitEffetc.hitAudioClip);
        }
    }



    /// <summary>
    /// 技能的特效
    /// </summary>
    /// <param name="skillObj"></param>
    private void SpawnSkill(SkillSpawnObj skillObj)
    {
        if (skillObj != null && (skillObj.prefab.prefabName != null && skillObj.prefab.folderName != null))
        {
            StartCoroutine(DoSpawnSkill(skillObj));
        }
    }
    private IEnumerator DoSpawnSkill(SkillSpawnObj skillObj)
    {

        yield return new WaitForSeconds(skillObj.Time);
        //直接生成预制体
        //GameObject skillPrefab = Instantiate(skillObj.prefab,null);
        //使用对象池生成预制体
        if (skillObj.prefab.folderName != "" && skillObj.prefab.prefabName != "")
        {
            Debug.Log(skillObj.prefab.folderName + "和" + skillObj.prefab.prefabName);
            GameObject skillPrefab = PoolManager.Instance.GetPoolObject(
            skillObj.prefab.prefabName, skillObj.prefab.folderName);
            Debug.Log("触发");
            skillPrefab.transform.position = _PlayerModle.transform.position +
            _PlayerModle.transform.forward * skillObj.position.z +
            _PlayerModle.transform.right * skillObj.position.x +
             _PlayerModle.transform.up * skillObj.position.y;
            //使用eulerAngles(欧拉角)来进行计算,移动和旋转都是模型层在做
            skillPrefab.transform.rotation = _PlayerModle.transform.rotation * Quaternion.Euler(skillObj.rotation);
        }
    }



    //技能生成
    private void SpawnAttackEffect(SkillSpawnObj skillObj)
    {
        if (skillObj != null && (skillObj.prefab.prefabName != "" && skillObj.prefab.folderName != ""))
        {
            StartCoroutine(DoAttackEffect(skillObj));
        }
    }
    private IEnumerator DoAttackEffect(SkillSpawnObj skillObj)
    {
        
        yield return new WaitForSeconds(skillObj.Time);
        //直接生成预制体
        //GameObject skillPrefab = Instantiate(skillObj.prefab,null);
        //使用对象池生成预制体
        if (skillObj.prefab.folderName != null && skillObj.prefab.prefabName != null)
        {
            GameObject skillPrefab = PoolManager.Instance.GetPoolObject(
            skillObj.prefab.prefabName, skillObj.prefab.folderName);
            skillPrefab.transform.position = _PlayerModle.transform.position +
            _PlayerModle.transform.forward * skillObj.position.z +
            _PlayerModle.transform.right * skillObj.position.x +
             _PlayerModle.transform.up * skillObj.position.y;
            //使用eulerAngles(欧拉角)来进行计算,移动和旋转都是模型层在做
            skillPrefab.transform.rotation = _PlayerModle.transform.rotation * Quaternion.Euler(skillObj.rotation);
        }
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






    #region 调试部分
    private void DrawDebugBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color, float duration = 5f)
    {
        // 绘制盒体的8个顶点
        Vector3[] points = new Vector3[8];

        // 计算盒体的8个角点
        points[0] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        points[1] = center + rotation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);
        points[2] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        points[3] = center + rotation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);
        points[4] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        points[5] = center + rotation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        points[6] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        points[7] = center + rotation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);

        // 绘制盒体的12条边
        Debug.DrawLine(points[0], points[1], color, duration);
        Debug.DrawLine(points[0], points[2], color, duration);
        Debug.DrawLine(points[0], points[4], color, duration);
        Debug.DrawLine(points[1], points[3], color, duration);
        Debug.DrawLine(points[1], points[5], color, duration);
        Debug.DrawLine(points[2], points[3], color, duration);
        Debug.DrawLine(points[2], points[6], color, duration);
        Debug.DrawLine(points[3], points[7], color, duration);
        Debug.DrawLine(points[4], points[5], color, duration);
        Debug.DrawLine(points[4], points[6], color, duration);
        Debug.DrawLine(points[5], points[7], color, duration);
        Debug.DrawLine(points[6], points[7], color, duration);
    }


    //可视化
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Vector3 checkPos = _PlayerModle.transform.position +
    //                      _PlayerModle.transform.forward * standAttckCongig[0].attackData.attackcheck.checkPos.z +
    //                      _PlayerModle.transform.up * standAttckCongig[0].attackData.attackcheck.checkPos.y +
    //                      _PlayerModle.transform.right * standAttckCongig[0].attackData.attackcheck.checkPos.x;
    //    Gizmos.DrawCube(checkPos,
    //        standAttckCongig[0].attackData.attackcheck.halfExtents);
    //}


    #endregion
}