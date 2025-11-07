using System;
using System.Runtime.CompilerServices;
using Unity.Android.Gradle.Manifest;
using Unity.VisualScripting;
using UnityEngine;


#region 弃用
//public class PlayerAirDownState : PlayerStateBase
//{
//    private enum AirDownChildState
//    {
//        Loop,
//        End
//    }

//    private float playerEndAnimationHeight = 1.5f;
//    private float endAnimationHeight = 0.6f;
//    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
//    private bool needEndAnimation;
//    private bool checkEndAnimation;
//    private AirDownChildState airState;
//    private float lastStateChangeTime;
//    private float minStateDuration = 0.2f;

//    private AirDownChildState AirState
//    {
//        get => airState;
//        set
//        {
//            if (Time.time - lastStateChangeTime < minStateDuration && airState != value)
//                return;

//            airState = value;
//            lastStateChangeTime = Time.time;

//            switch (airState)
//            {
//                case AirDownChildState.Loop:
//                    _player.PlayAnimation("JumpLoop");
//                    break;
//                case AirDownChildState.End:
//                    _player.PlayAnimation("JumpEnd");
//                    break;
//            }
//        }
//    }

//    public override void Enter()
//    {

//        AirState = AirDownChildState.Loop;
//        lastStateChangeTime = Time.time;

//        needEndAnimation = !Physics.Raycast(_player.transform.position + new Vector3(0, 0.5f, 0),
//           Vector3.down, playerEndAnimationHeight, groundLayerMask);
//    }

//    public override void Update()
//    {
//        switch (airState)
//        {
//            case AirDownChildState.Loop:
//                if (needEndAnimation)
//                {
//                    checkEndAnimation = Physics.Raycast(_player.transform.position + new Vector3(0, 0.5f, 0),
//                         Vector3.down, endAnimationHeight, groundLayerMask);

//                    if (checkEndAnimation)
//                    {
//                        AirState = AirDownChildState.End;
//                        return;
//                    }
//                }
//                else
//                {
//                    // 更严格的落地检测
//                    bool isProperlyGrounded = _player._CharacterController.isGrounded &&
//                        Physics.CheckSphere(_player.transform.position, 0.2f, groundLayerMask);

//                    if (isProperlyGrounded)
//                    {
//                        _player.ChangeState(PlayerStateType.Idle);
//                        return;
//                    }
//                }
//                AirControll();
//                break;

//            case AirDownChildState.End:
//                if (CheckAnimatorStateName("JumpEnd", out float time))
//                {
//                    if (time >= 0.7f)
//                    {
//                        bool isCast = Physics.CheckSphere(_player.transform.position, 1f, groundLayerMask);
//                        // 严格的落地检测，避免状态震荡
//                        bool isStableGrounded = isCast || _player._CharacterController.isGrounded;
//                        Debug.Log($"范围检测: {isCast}, 控制器落地: {_player._CharacterController.isGrounded}");

//                        if (isCast)
//                        {
//                            _player.ChangeState(PlayerStateType.Idle);
//                        }
//                        else
//                        {
//                            AirState = AirDownChildState.Loop;
//                        }
//                    }
//                    else
//                    {
//                        AirControll();
//                    }
//                }
//                break;
//        }
//    }

//    private void AirControll()
//    {
//        float vertical = Input.GetAxis("Vertical");
//        float horizontal = Input.GetAxis("Horizontal");
//        Vector3 motion = Vector3.zero;

//        // 只在空中时应用重力
//        if (!_player._CharacterController.isGrounded)
//        {
//            motion.y = _player._gravity * Time.deltaTime;
//        }

//        if (vertical != 0 || horizontal != 0)
//        {
//            float y = Camera.main.transform.rotation.eulerAngles.y;
//            Vector3 input = new Vector3(horizontal, 0, vertical);
//            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;

//            motion.x = _player.moveSpeedForJump * Time.deltaTime * targetDir.x;
//            motion.z = _player.moveSpeedForJump * Time.deltaTime * targetDir.z;

//            _player._PlayerModle.transform.rotation = Quaternion.Slerp(_player._PlayerModle.transform.rotation,
//                Quaternion.LookRotation(targetDir), Time.deltaTime * _player._rotationSpeed);
//        }

//        _player._CharacterController.Move(motion);
//    }

//    private void OnDrawGizmos()
//    {
//        if (UnityEngine.Application.isPlaying)
//        {
//            // 绘制CheckSphere的范围
//            Gizmos.color = Physics.CheckSphere(_player.transform.position, 0.2f, groundLayerMask) ? Color.green : Color.red;
//            Gizmos.DrawWireSphere(_player.transform.position, 0.2f);
//        }
//    }
//}
#endregion
public class PlayerAirDownState : PlayerStateBase
{
    private enum AirDownChildState
    {
        Loop,
        End
    }

    private float playerEndAnimationHeight = 1.5f;
    private float endAnimationHeight = 1.2f;
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    private bool needEndAnimation;
    private bool checkEndAnimation;
    //private bool hasLoopAnimationCompleted;
    private AirDownChildState airState;
    private AirDownChildState AirState
    {
        get => airState;
        set 
        {
            airState = value;
            //hasLoopAnimationCompleted = false;
            switch (airState)
            {
                case AirDownChildState.Loop:
                    _player.PlayAnimation("JumpLoop");
                    break;
                case AirDownChildState.End:
                    _player.PlayAnimation("JumpEnd");
                    break;
            }
        }
    }

    public override void Enter()
    {
        _player._CharacterController.Move(new Vector3(0, _player._gravity * Time.deltaTime, 0));
        AirState = AirDownChildState.Loop;

        needEndAnimation = !Physics.Raycast(_player.transform.position + new Vector3(0, 0.5f, 0),
           Vector3.down, playerEndAnimationHeight, groundLayerMask);
           //hasLoopAnimationCompleted = false;
    }

    public override void Update()
    {
        #region 状态混乱时的解决方案
        //Debug.Log("111111111111");
        //if (_player._stateMachine.CurrentStateType != typeof(PlayerAirDownState))
        //{
        //    return; // 如果状态已切换，立即退出
        //}
        //CheckAnimatorStateName(airState.ToString(), out float animTime);
        //CheckLayerMask();
        #endregion

        switch (airState)
        {
            case AirDownChildState.Loop:

                #region 强条件检测，效果不好

                //if (CheckAnimatorStateName("JumpLoop", out float loopTime))
                //{
                //    // 如果动画播放完成一次（normalizedTime >= 1）
                //    if (loopTime >= 1.0f && !hasLoopAnimationCompleted)
                //    {
                //        hasLoopAnimationCompleted = true;
                //    }

                //    // 只有动画播放完成后才进行状态检测
                //    if (hasLoopAnimationCompleted)
                //    {
                //        AirControll();
                //        if (needEndAnimation)
                //        {
                //            checkEndAnimation = Physics.Raycast(_player.transform.position + new Vector3(0, 0.5f, 0),
                //                 _player.transform.up * -1, endAnimationHeight, groundLayerMask);
                //            if (checkEndAnimation)
                //            {
                //                AirState = AirDownChildState.End;
                //            }
                //            else if (_player._CharacterController.isGrounded)
                //            {
                //                //SyncModelToController();
                //                _player.ChangeState(PlayerStateType.Idle);
                //                return;
                //            }
                //        }
                //        else
                //        {
                //            if (_player._CharacterController.isGrounded)
                //            {
                //                Debug.Log("22222222222");
                //                _player.ChangeState(PlayerStateType.Idle);
                //                //_player.ForceAnimationTransition("Idle");
                //                return;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        // 动画播放期间只应用重力和简单移动
                //        AirControll();
                //        //SimpleAirControll();
                //    }
                //}
                //break;
                #endregion

                #region 常规条件检测，效果较好
                
                if (needEndAnimation)
                {
                    checkEndAnimation = Physics.Raycast(_player.transform.position + new Vector3(0, 0.5f, 0),
                         _player.transform.up * -1, endAnimationHeight + 0.5f, groundLayerMask);
                    if (checkEndAnimation)
                    {
                        AirState = AirDownChildState.End;
                    }
                    else if (_player._CharacterController.isGrounded)
                    {
                        _player.ChangeState(PlayerStateType.Idle);
                        return;
                    }
                }
                else
                {
                    if (_player._CharacterController.isGrounded)
                    {
                        _player.ChangeState(PlayerStateType.Idle);
                        return;
                    }
                }
                AirControll();
                break;
            #endregion

            case AirDownChildState.End:
                //_player._CharacterController.Move(Vector3.down*Time.deltaTime*9.8f);
                if (CheckAnimatorStateName("JumpEnd",out float time))
                {
                    if (time >= 0.15f)
                    {
                        if (!_player._CharacterController.isGrounded)
                        {
                            AirState = AirDownChildState.Loop;
                        }
                        else
                        {
                            _player.ChangeState(PlayerStateType.Idle);
                        }

                    }
                    //else if(time < 0.5f)
                    //{

                    //    //AirControll();
                    //    //SimpleAirControll();
                    //}
                }
                //移动逻辑主要看游戏中具体效果，结合动画过渡时间
                AirControll();
                break;
        }
    }

    private void AirControll()
    {
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        Vector3 motion = new Vector3(0, _player._gravity * Time.deltaTime, 0);
        if (vertical != 0 || horizontal != 0)
        {
            float y = Camera.main.transform.rotation.eulerAngles.y;
            Vector3 input = new Vector3(horizontal, 0, vertical);
            Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;

            motion.x = _player.moveSpeedForJump * Time.deltaTime * targetDir.x;
            motion.z = _player.moveSpeedForJump * Time.deltaTime*targetDir.z;
            
            _player._PlayerModle.transform.rotation = Quaternion.Slerp(_player._PlayerModle.transform.rotation,
                Quaternion.LookRotation(targetDir), Time.deltaTime * _player._rotationSpeed);
        }
        _player._CharacterController.Move(motion);
    }

    void CheckLayerMask()
    {
        Debug.Log("是否到地面："+_player._CharacterController.isGrounded);
        Debug.Log("是否需要播放结束动画："+needEndAnimation);
    }
}

