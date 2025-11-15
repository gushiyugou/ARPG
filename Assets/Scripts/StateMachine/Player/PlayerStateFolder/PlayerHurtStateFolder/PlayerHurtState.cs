using UnityEngine;
using System.Collections;

public class PlayerHurtState : PlayerStateBase
{
    public enum HurtChildState
    {
        Normal,
        Down,
        Rise
    }

    private SkillHitData hitData => _player.HitData;
    private float currentHurtTime = 0;
    private Coroutine repelCoroutine;
    private ISkillOwner sourceTransform => _player.HitSource;

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
                    _player.PlayAnimation("Hurt");
                    break;
                case HurtChildState.Down:
                    _player.PlayAnimation("Down");
                    break;
                //case HurtChildState.Rise:
                //    _player.PlayAnimation("Rise");
                //    break;
            }
        }
    }

    public override void Enter()
    {
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
            _player.characterController.Move(new Vector3(0, _player._gravity * Time.deltaTime, 0));
        }
        currentHurtTime += Time.deltaTime;
        switch (HurtState)
        {
            case HurtChildState.Normal:
                //if (hitData == null) return;
                if (currentHurtTime >= hitData.stiffTime && repelCoroutine == null)
                {
                    _player.Model._Animator.SetBool("IsIdle",true);
                    _player.ChangeState(PlayerStateType.Idle);
                }
                break;
            case HurtChildState.Down:
                Vector3 playerPos = sourceTransform.ModelTransform.position;
                _player.Model.transform.LookAt(new Vector3(playerPos.x, _player.ModelTransform.position.y, playerPos.z));
                //_player.Model.transform.LookAt(sourceTransform.ModelTransform);
                if (currentHurtTime >= hitData.stiffTime && repelCoroutine == null)
                {
                    //HurtState = HurtChildState.Rise;
                    _player.Model._Animator.SetBool("IsIdle", true);
                    _player.ChangeState(PlayerStateType.Idle);
                }
                break;
            //case HurtChildState.Rise:
            //    _player.Model.transform.LookAt(sourceTransform.ModelTransform);
            //    if (CheckAnimatorStateName("Rise",out float time) && time > 0.99f)
            //    {
            //        _player.ChangeState(PlayerStateType.Idle);
            //    } 
            //    break;
        }

        


    }

    private IEnumerator DoRepel(float time,Vector3 velocity)
    {
        float currentTime = 0f;
        time = time==0? 0.00001f:time;
        //Vector3 repelDir = sourceTransform.ModelTransform.TransformDirection(velocity);
        Vector3 targetPos = sourceTransform.ModelTransform.TransformPoint(velocity);
        Vector3 dir = targetPos - _player.transform.position;
        while(currentTime< time)
        {
            _player.characterController.Move(dir / time * Time.deltaTime);
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
