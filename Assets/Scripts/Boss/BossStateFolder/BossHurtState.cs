using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

public class BossHurtState : BossStateBase
{
    public enum HurtChildState
    {
        Normal,
        Down,
        Rise
    }

    private SkillHitData hitData => boss.HitData;
    private float currentHurtTime = 0;
    private Coroutine repelCoroutine;
    Vector3 pos;
    private ISkillOwner sourceTransform =>boss.HitSource;

    private HurtChildState hurtState;
    public HurtChildState HurtState
    {
        get=> hurtState;
        set
        {
            hurtState = value;
            switch (hurtState)
            {
                case HurtChildState.Normal:
                    boss.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    boss.transform.LookAt(new Vector3(pos.x, sourceTransform.ModelTransform.position.y, pos.z));
                    boss.PlayAnimation("Down");
                    break;
                case HurtChildState.Rise:
                    boss.PlayAnimation("Rise");
                    break;
            }
        }
    }

    public override void Enter()
    {
        pos = sourceTransform.ModelTransform.position;
        currentHurtTime = 0;
        HurtState = hitData.isDown ? HurtChildState.Down : HurtChildState.Normal;

        if (hitData.repelDegree != Vector3.zero)
        {
            repelCoroutine = MonoManager.Instance.StartCoroutine(DoRepel(hitData.repelTime, hitData.repelDegree));
        }
    }


    public override void Update()
    {
        if(repelCoroutine == null)
        {
            boss.characterController.Move(new Vector3(0, boss._gravity * Time.deltaTime, 0));
        }
        currentHurtTime += Time.deltaTime;
        pos = sourceTransform.ModelTransform.position;
        switch (HurtState)
        {
            case HurtChildState.Normal:
                //if (hitData == null) return;
                if (currentHurtTime >= hitData.stiffTime && repelCoroutine == null)
                {
                    boss.ChangeState(BossStateType.Idle);
                }
                break;
            case HurtChildState.Down:
                
                boss.transform.LookAt(new Vector3(pos.x, sourceTransform.ModelTransform.position.y, pos.z));
                //boss.transform.LookAt(sourceTransform.ModelTransform);
                if (currentHurtTime >= hitData.stiffTime && repelCoroutine == null)
                {
                    HurtState = HurtChildState.Rise;
                }
                break;
            case HurtChildState.Rise:
                boss.transform.LookAt(new Vector3(pos.x, sourceTransform.ModelTransform.position.y, pos.z));
                //boss.transform.LookAt(sourceTransform.ModelTransform);
                if (CheckAnimatorStateName("Rise",out float time) && time > 0.99f)
                {
                    boss.ChangeState(BossStateType.Idle);
                } 
                break;
        }
        
    }

    private IEnumerator DoRepel(float time,Vector3 velocity)
    {
        float currentTime = 0f;
        time = time==0? 0.00001f:time;
        //Vector3 repelDir = sourceTransform.ModelTransform.TransformDirection(velocity);
        Vector3 targetPos = sourceTransform.ModelTransform.TransformPoint(velocity);
        Vector3 dir = targetPos - boss.transform.position;
        while (currentTime< time)
        {
            //boss.characterController.Move(repelDir / time * Time.deltaTime);
            boss.characterController.Move(dir / time * Time.deltaTime);
            currentTime += Time.deltaTime;
            yield return null;
        }

        repelCoroutine =null;
    }

    public override void Exit()
    {
        if(repelCoroutine != null)
        {
            MonoManager.Instance.StopCoroutine(repelCoroutine);
            repelCoroutine = null;
        }
        
    }
}
