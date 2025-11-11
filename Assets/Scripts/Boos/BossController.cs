using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class BossController : MonoBehaviour, IHurt, IStateMachineOwner,ISkillOwner
{
   
    [SerializeField]private BossModle bossModle;
    public BossModle Model { get => bossModle; }
    public Transform ModelTransform => Model.transform;
    [SerializeField] private CharacterController _characterController;
    public CharacterController characterController { get => _characterController; }
    private StateMachine stateMachine;
    public List<string> enemyTagList;

    public SkillHitData HitData { get; private set; }
    public ISkillOwner HitSource { get; private set; }

    [SerializeField, Header("ÍÏÎ²²å¼þ")] private MeleeWeaponTrail weaponTrail;

    private void Start()
    {
        Model.OnInit(this, enemyTagList);
        stateMachine = new StateMachine();
        bossModle = GetComponentInChildren<BossModle>();
        stateMachine.Init(this);
        ChangeState(BossStateType.Idle);
    }

    


    //ÇÐ»»×´Ì¬
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

    

    public void Hurt(SkillHitData hitData, ISkillOwner hitSource)
    {
        HitData = hitData;
        HitSource = hitSource;
        Debug.Log("bossÊÜÉË");
        ChangeState(BossStateType.Hurt, true);
    }


    public void PlayAnimation(string animationName, float fixedTransitionDuration = 0.25f)
    {
        bossModle._Animator.CrossFadeInFixedTime(animationName, fixedTransitionDuration);
    }


    #region UnityEditor À©Õ¹¹¦ÄÜ
#if UNITY_EDITOR
    [ContextMenu("SetHurtCollider")]
    private void SetHurtCollider()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        for(int i =0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<WeaponController>() == null)
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("HurtCollider");
                colliders[i].gameObject.tag = "Enemy";
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
#endif
    #endregion

    public void StartSkillHit(int weaponIndex)
    {
        weaponTrail.Emit = true;
    }

    public void StopSkillHit(int weaponIndex)
    {
        weaponTrail.Emit = true;
    }

    public void SkillCanSwitch()
    {
        
    }

    public void OnHit(IHurt target, Vector3 hitPosition)
    {
        
    }

    public void OnFootStep()
    {
        
    }

}

