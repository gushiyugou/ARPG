

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour, IStateMachineOwner, ISkillOwner,IHurt
{
    #region 控制器基础组件
    public readonly float _gravity = -9.8f;
    [Header("基础组件")]
    [SerializeField] protected ModelBase model;
    public ModelBase Model { get => model; }


    public Transform ModelTransform => Model.transform;
    [SerializeField] protected CharacterController _characterController;


    public CharacterController characterController { get => _characterController; }
    protected StateMachine stateMachine;

    [SerializeField] protected AudioSource audioSource;
    #endregion

    //TODO 测试的配置信息，直接拖拽，后续会改
    //public SkillConfig _skillConfig;
    [Header("敌人Tag列表")] public List<string> enemyTagList;
    protected List<IHurt> enemyList = new List<IHurt>();


    //[Header("扇形攻击检测")]
    protected SectorAttackDetector attackDetector;
    //[SerializeField] private float baseAttackDamage = 10f;


    #region 武器相关
    //拖尾组件
    [SerializeField, Header("拖尾插件")] protected MeleeWeaponTrail weaponTrail;
    public Transform weapon;

    #endregion
    public virtual void Init()
    {
        Model.OnInit(this, enemyTagList);
        stateMachine = new StateMachine();
        stateMachine.Init(this);
        _characterController = GetComponent<CharacterController>();
        CanSwitchSkill = true;
    }


    #region 技能相关
    /// <summary>
    /// 技能配置数组
    /// </summary>
    [Header("技能配置")]
    public SkillConfig[] standAttckCongigs;
    public SkillConfig CurrentSkillConfig { get; protected set; }
    protected int currentHitIndex = 0;
    public int currentHitWeapIndex;
    public bool CanSwitchSkill { get; protected set; }

    public SkillHitData HitData { get; protected set; }
    public ISkillOwner HitSource { get; protected set; }



    /// <summary>
    /// 技能的攻击
    /// </summary>
    /// <param name="skillConfig"></param>
    public virtual void StartAttack(SkillConfig skillConfig)
    {
        CanSwitchSkill = false;

        CurrentSkillConfig = skillConfig;
        //表示是新技能的开始
        currentHitIndex = 0;
        //播放动画
        PlayAnimation(CurrentSkillConfig.AnimationName);
        //技能释放时角色音效
        PlayAudio(CurrentSkillConfig.releaseData.skillAudio);
        //技能释放时角色的特效
        SpawnSkill(CurrentSkillConfig.releaseData.effectObj);
        //击中检测

        //伤害传递
    }



    //攻击发起时，（实际的攻击动作，去掉了前摇和后摇的时间）
    public virtual void StartSkillHit(int weaponIndex)
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



    //技能结束击中
    public virtual void StopSkillHit(int weaponIndex)
    {
        // 添加状态检查
        //if (!CanSwitchSkill)
        //{
        //    //Debug.LogWarning("接收到StopSkillHit但不在技能状态中，已忽略");
        //    return;
        //}
        currentHitIndex += 1;
        //如果用到了currentHitIndex，则currentHitIndex需要-1，也可以在攻击结束时，currentHitIndex+1
        weaponTrail.Emit = false;
    }



    //技能后摇的部分
    public virtual void SkillCanSwitch()
    {
        CanSwitchSkill = true;
    }
    public virtual void OnHit(IHurt target, Vector3 hitPosition)
    {

        //Debug.Log("角色控制：我攻击到了" + ((Component)target).gameObject.name);
        //OnHit在Stop之后执行，所以索引要减一
        SkillAttackData skillData = CurrentSkillConfig.attackData[currentHitIndex];
        //PlayAudio(skillData.attackAudio);

        //对IHurt传递伤害数据
        //TODO:后续做特殊情况的处理
        if (target.Hurt(skillData.hitDatat, this))
        {
            StartCoroutine(DoSkillHitEffect(skillData.hitEffect.skillSpawnObj, hitPosition));
            DoFreezeFrameTime(skillData.FreezeFrameTime);

            DoFreezeGame(skillData.FreezeGameTime);
            Debug.Log("击中");
        }
        else
        {
            StartCoroutine(DoSkillHitEffect(skillData.hitEffect.failSkillSpawnObj, hitPosition));
        }

    }


    /// <summary>
    /// 攻击检测
    /// </summary>
    /// <param name="skillConfig"></param>
    public virtual void AttackEffectCheck(SkillAttackData attackData)
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




    /// <summary>
    /// 卡肉效果
    /// </summary>
    /// <param name="force"></param>
    public  void DoFreezeFrameTime(float time)
    {
        StartCoroutine(StartFreezeFrameTime(time));
    }
    public IEnumerator StartFreezeFrameTime(float time)
    {
        model._Animator.speed = 0;
        yield return new WaitForSeconds(time);
        model._Animator.speed = 1;
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
    protected IEnumerator DoSkillHitEffect(SkillSpawnObj hitEffetc, Vector3 pos)
    {
        if (hitEffetc == null) yield break;

        if (hitEffetc != null && hitEffetc.prefab != null)
        {
            yield return new WaitForSeconds(hitEffetc.Time);
            //直接生成预制体
            //GameObject temp = Instantiate(hitEffetc.skillSpawnObj.prefab);
            //使用对象池生成预制体
            GameObject temp = PoolManager.Instance.GetPoolObject(hitEffetc.prefab.prefabName,
                hitEffetc.prefab.folderName);
            Debug.Log("触发了");

            temp.transform.position = pos + hitEffetc.position;
            //temp.transform.LookAt(Camera.main.transform);
            temp.transform.eulerAngles = pos + hitEffetc.rotation;
            PlayAudio(hitEffetc.spawnAudio);
        }
    }



    /// <summary>
    /// 技能的特效
    /// </summary>
    /// <param name="skillObj"></param>
    protected void SpawnSkill(SkillSpawnObj skillObj)
    {
        if (skillObj != null && (skillObj.prefab.prefabName != null && skillObj.prefab.folderName != null))
        {
            StartCoroutine(DoSpawnSkill(skillObj));
        }
    }
    protected IEnumerator DoSpawnSkill(SkillSpawnObj skillObj)
    {

        yield return new WaitForSeconds(skillObj.Time);
        //直接生成预制体
        //GameObject skillPrefab = Instantiate(skillObj.prefab,null);
        //使用对象池生成预制体
        if (skillObj.prefab.folderName != "" && skillObj.prefab.prefabName != "")
        {
            GameObject skillPrefab = PoolManager.Instance.GetPoolObject(
            skillObj.prefab.prefabName, skillObj.prefab.folderName);
            skillPrefab.transform.position = Model.transform.position +
                Model.transform.TransformDirection(skillObj.position);
            //_PlayerModle.transform.forward * skillObj.position.z +
            //_PlayerModle.transform.right * skillObj.position.x +
            // _PlayerModle.transform.up * skillObj.position.y;
            skillPrefab.transform.localScale = skillObj.scale;
            //使用eulerAngles(欧拉角)来进行计算,移动和旋转都是模型层在做
            skillPrefab.transform.rotation = Model.transform.rotation * Quaternion.Euler(skillObj.rotation);
        }
    }



    //技能生成
    protected void SpawnAttackEffect(SkillSpawnObj skillObj)
    {
        if (skillObj != null && (skillObj.prefab.prefabName != "" && skillObj.prefab.folderName != ""))
        {
            StartCoroutine(DoAttackEffect(skillObj));
        }
    }
    protected IEnumerator DoAttackEffect(SkillSpawnObj skillObj)
    {

        yield return new WaitForSeconds(skillObj.Time);
        //直接生成预制体
        //GameObject skillPrefab = Instantiate(skillObj.prefab,null);
        //使用对象池生成预制体
        if (skillObj.prefab.folderName != null && skillObj.prefab.prefabName != null)
        {
            GameObject skillPrefab = PoolManager.Instance.GetPoolObject(
            skillObj.prefab.prefabName, skillObj.prefab.folderName);
            skillPrefab.transform.position = Model.transform.position +
                Model.transform.TransformDirection(skillObj.position);
            //_PlayerModle.transform.forward * skillObj.position.z +
            //_PlayerModle.transform.right * skillObj.position.x +
            // _PlayerModle.transform.up * skillObj.position.y;

            skillPrefab.transform.localScale = skillObj.scale;
            //使用eulerAngles(欧拉角)来进行计算,移动和旋转都是模型层在做
            skillPrefab.transform.rotation = Model.transform.rotation * Quaternion.Euler(skillObj.rotation);
        }
    }



    public void OnSkillOver()
    {
        CanSwitchSkill = true;
    }

    public void OnFootStep()
    {
        throw new System.NotImplementedException();
    }
    #endregion


    #region 动画播放相关
    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="animationName">动画名字</param>
    /// <param name="fixedTransitionDuration">过渡时间</param>
    public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.25f)
    {
        Model._Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);

    }



    /// <summary>
    /// 立即播放动画
    /// </summary>
    /// <param name="animationName">动画名字</param>
    /// <param name="layer">动画的层级</param>
    /// <param name="num">开始播放的帧数</param>
    public void PlayAnimationImmediately(string animationName, int layer, int num)
    {
        Model._Animator.Play(animationName, layer, num);
    }




    /// <summary>
    /// 检查当前是否正在播放指定动画
    /// </summary>
    public bool IsPlayingAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = Model._Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }




    /// <summary>
    /// 强制动画过渡
    /// </summary>
    public void ForceAnimationTransition(string targetAnimation, float transitionDuration = 0.1f)
    {
        Model._Animator.CrossFade(targetAnimation, transitionDuration);
    }


    /// <summary>
    /// 动画回调事件
    /// </summary>
    protected void OnAnimatorMove()
    {
        // 应用根运动位移
        _characterController.Move(Model._Animator.deltaPosition);
        transform.rotation *= Model._Animator.deltaRotation;

    }

    #endregion







    //public void OnFootStep()
    //{
    //    //_audioSource.PlayOneShot(footStepAudioClips[Random.Range(0, footStepAudioClips.Length)]);
    //}



    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip != null)
            audioSource.PlayOneShot(audioClip);

    }

    public virtual bool SetHurtData(SkillHitData hitData, ISkillOwner hitSource)
    {
        HitData = hitData;
        HitSource = hitSource;
        return true;
    }
    public abstract bool Hurt(SkillHitData hitData, ISkillOwner hitSource);

    #region 调试部分
    protected void DrawDebugBox(Vector3 center, Vector3 halfExtents, Quaternion rotation, Color color, float duration = 5f)
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
    //    Vector3 checkPos = Model.transform.position + Model.transform.TransformDirection(CurrentSkillConfig.attackData[currentHitIndex].attackcheck.checkPos);
    //                      //Model.transform.forward * standAttckCongigs[1].attackData[currentHitIndex].attackcheck.checkPos.z +
    //                      //Model.transform.up * standAttckCongigs[1].attackData[currentHitIndex].attackcheck.checkPos.y +
    //                      //Model.transform.right * standAttckCongigs[1].attackData[currentHitIndex].attackcheck.checkPos.x;
    //    Gizmos.DrawCube(checkPos,
    //        CurrentSkillConfig.attackData[currentHitIndex].attackcheck.halfExtents);
    //}


    #endregion
}
