using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class PlayerDefenceState : PlayerStateBase
{
    public enum DefenceChildState
    {
        Start,
        Hold,
        WaitCounterattack,
        Counterattack,
        End
    }

    private DefenceChildState childState;
    public DefenceChildState ChildState
    {
        get => childState;
        set
        {
            childState = value;
            switch (childState)
            {
                case DefenceChildState.Start:
                    _player.PlayAnimation("StartDefence");
                    break;
                case DefenceChildState.Hold:
                    break;
                case DefenceChildState.WaitCounterattack:
                    waitCounterattackTimeCoroutine = MonoManager.Instance.StartCoroutine(WaitCounterattackTime());
                    break;
                case DefenceChildState.Counterattack:
                    _player.StartAttack(_player.counterattackConfig);
                    break;
                case DefenceChildState.End:
                    _player.PlayAnimation("EndDefence");
                    break;
            }
        }
    }
   
    public override void Enter()
    {
        _player.Model.SetRootMotionAction(OnRootMotion);
        ChildState = DefenceChildState.Start;
    }

    public void Hurt()
    {
        if(ChildState == DefenceChildState.Hold)
        {
            ChildState = DefenceChildState.WaitCounterattack;
        }
    }

    public override void Update()
    {
        switch (childState)
        {
            case DefenceChildState.Start:
                if(CheckAnimatorStateName("StartDefence",out float animTime) && animTime >= 1f)
                {
                    ChildState = DefenceChildState.Hold;
                }
                break;
            case DefenceChildState.Hold:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    ChildState = DefenceChildState.End;
                }
                break;
            case DefenceChildState.WaitCounterattack:
                if (Input.GetMouseButtonDown(0))
                {
                    MonoManager.Instance.StopCoroutine(WaitCounterattackTime());
                    waitCounterattackTimeCoroutine = null;
                    ChildState = DefenceChildState.Counterattack;
                }
                else if (Input.GetKeyUp(KeyCode.F))
                {
                    MonoManager.Instance.StopCoroutine(WaitCounterattackTime());
                    waitCounterattackTimeCoroutine = null;
                    ChildState = DefenceChildState.End;
                }
                break;
            case DefenceChildState.Counterattack:
                if(CheckAnimatorStateName(_player.counterattackConfig.AnimationName,out float attackAnimaTime) && attackAnimaTime >= 1f)
                {
                    _player.Model._Animator.SetBool("IsIdle", true);
                    _player.ChangeState(PlayerStateType.Idle);
                }
                else if(_player.CanSwitchSkill && Input.GetMouseButton(0))
                {
                    _player.ChangeState(PlayerStateType.AtkNormal1);
                }
                break;
            case DefenceChildState.End:
                if (CheckAnimatorStateName("EndDefence", out float endAnimTime) && endAnimTime >= 1f)
                {
                    _player.Model._Animator.SetBool("IsIdle", true);
                    _player.ChangeState(PlayerStateType.Idle);
                }
                break;
        }
    }

    

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = _player._gravity * Time.deltaTime;
        _player.characterController.Move(deltaPosition);
    }


    private Coroutine waitCounterattackTimeCoroutine;

    private IEnumerator WaitCounterattackTime()
    {
        yield return new WaitForSeconds(_player.waitCounterattackTime);
        childState = DefenceChildState.Hold;
        waitCounterattackTimeCoroutine = null;
    }

    public override void Exit()
    {
        _player.Model.ClearRootMotionAction();
    }
}
