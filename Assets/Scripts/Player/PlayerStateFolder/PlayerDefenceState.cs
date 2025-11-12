using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class PlayerDefenceState : PlayerStateBase
{
    public enum DefenceChildState
    {
        Start,
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
                case DefenceChildState.WaitCounterattack:
                    break;
                case DefenceChildState.Counterattack:
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

    public override void Update()
    {
        switch (childState)
        {
            case DefenceChildState.Start:
                if(CheckAnimatorStateName("StartDefence",out float animTime) && animTime >= 1f)
                {
                    ChildState = DefenceChildState.WaitCounterattack;
                }
                break;
            case DefenceChildState.WaitCounterattack:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    ChildState = DefenceChildState.End;
                }
                break;
            case DefenceChildState.Counterattack:
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

    public override void Exit()
    {
        _player.Model.ClearRootMotionAction();
    }
}
