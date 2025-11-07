using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerAtkNormal1State : PlayerStateBase
{
    //private float rotSpeed = 2f;
    private bool isAtk = true;
    public override void Enter()
    {
        //处理攻击方向
       
        _player._PlayerModle.SetRootMotionAction(OnRootMotion);
        StandAttck();
    }

    private void StandAttck()
    {
        //TODO：实现连续普攻
        _player.StartAttack(_player.standAttckCongig[0]);
    }
    public override void Update()
    {
        
        FaceMouseDirectionAndAttack();
        if (CheckAnimatorStateName(_player.standAttckCongig[0].AnimationName, out float animTime) && animTime >= 0.55f)
        {
            isAtk = false;
            _player.ChangeState(PlayerStateType.Idle);
            return;
        }
    }

    
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = _player._gravity * Time.deltaTime;
        _player._CharacterController.Move(deltaPosition);
    }

    public override void Exit()
    {
        _player.PlayAnimation("Idle");
    }


    public void FaceMouseDirectionAndAttack()
    {

        //// 即使有移动输入，也优先面向鼠标方向
        //Vector3 mouseDir = GetMouseWorldDirection();
        //Quaternion qua = Quaternion.LookRotation(mouseDir);
        //if (mouseDir != Vector3.zero)
        //{
        //    _player.transform.rotation = Quaternion.Lerp(_player.transform.rotation, qua, Time.deltaTime*_player.rotSpeed);
        //}
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            //处理旋转的问题
            Vector3 input = new Vector3(horizontal, 0, vertical);
            float y = Camera.main.transform.eulerAngles.y;

            //让四元数与向量相乘，表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
            Vector3 targetDiretion = Quaternion.Euler(0, y, 0) * input;
            _player._PlayerModle.transform.rotation = Quaternion.Slerp(
                _player._PlayerModle.transform.rotation, Quaternion.LookRotation(targetDiretion),
                Time.deltaTime * _player.rotSpeed);
        }
    }
}
