using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSidestepReverseState : PlayerStateBase
{
    public override void Enter()
    {
        _player.PlayAnimation("SidestepReverse");
        SetRootAnima(true);
        _player._PlayerModle.SetRootMotionAction(OnRootMontion);

    }

    public override void Update()
    {
        if (CheckAnimatorStateName("SidestepReverse", out float animTime))
        {
            if (animTime > 0.4f)
            {
                UpdataGravity();
            }
            if (animTime > 0.8f)
            {
                _player.ChangeState(PlayerStateType.Idle);
            }
        }
        
    }

    private void OnRootMontion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition *= Mathf.Clamp(moveStatePower, 1, 2);
        //deltaPosition.y = _player._gravity * Time.deltaTime;
        _player._CharacterController.Move(deltaPosition);
    }
}
