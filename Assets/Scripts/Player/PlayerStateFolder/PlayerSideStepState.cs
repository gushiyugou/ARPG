using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSideStepState : PlayerStateBase
{
    Coroutine rotateCoroutine;
    private bool isRotate = false;
    public override void Enter()
    {
        SetRootAnima(true);
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if(h != 0 || v != 0)
        {
            //if()
            Vector3 rotDir = new Vector3(h, 0, v);
            rotateCoroutine = MonoManager.Instance.StartCoroutine(DoRotation(rotDir));
        }
        else
        {
            _player.Model.SetRootMotionAction(OnRootMontion);
            _player.PlayAnimation("Sidestep");
        }
    }

    private IEnumerator DoRotation(Vector3 rotDir)
    {
        isRotate = true;
        float y = Camera.main.transform.eulerAngles.y;
        Vector3 targetDir = Quaternion.Euler(0,y,0) * rotDir;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);
        float changeValue = 0;
        while(changeValue < 1)
        {
            changeValue += Time.deltaTime * 50;
            _player.Model.transform.rotation = Quaternion.Slerp(
              _player.Model.transform.rotation, targetRot, changeValue
            );
            yield return null;
        }
        isRotate = false;
        _player.Model.SetRootMotionAction(OnRootMontion);
        _player.PlayAnimation("Sidestep");
    }

    public override void Update()
    {
        if (isRotate) return;
        if(CheckAnimatorStateName("Sidestep",out float animTime))
        {
            //if (animTime > 0.5f)
            //{
            //    UpdataGravity();
            //}
            if (animTime > 0.65f)
            {
                _player.ChangeState(PlayerStateType.Idle);
            }
        }
    }

    public override void Exit()
    {
        moveStatePower = 0;
        _player.Model.ClearRootMotionAction();
        if(rotateCoroutine != null) MonoManager.Instance.StopCoroutine(rotateCoroutine);
    }

    private void OnRootMontion(Vector3 deltaPosition,Quaternion deltaRotation)
    {
        deltaPosition *= Mathf.Clamp(moveStatePower, 1, 2);
        //deltaPosition.y = _player._gravity *Time.deltaTime;
        _player._CharacterController.Move(deltaPosition);
    }
}
