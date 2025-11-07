using UnityEngine;

/// <summary>
/// 在使用单个跳跃状态时，只需要将JumpStart动画换成一个完整的跳跃状态时的动画即可
/// </summary>

//public class PlayerJumpState : PlayerStateBase
//{
//    private float jumpStartTime;
//    private float currentJumpVelocity;
//    private bool isJumping = true;

//    public override void Enter()
//    {
//        _player.PlayAnimation("JumpStart");
//        _player._PlayerModle.SetRootMotionAction(OnRootMotion);
//        jumpStartTime = Time.time;
//        currentJumpVelocity = _player.jumpStartSpedd; // 初始跳跃速度

//        // 禁用CharacterController的重力检测，由我们自己控制
//        _player._CharacterController.detectCollisions = false;
//    }

//    public override void Update()
//    {
//        // 模拟跳跃物理
//        if (isJumping)
//        {
//            // 应用重力
//            currentJumpVelocity -= _player._gracity * Time.deltaTime;

//            // 计算帧位移
//            float verticalMove = currentJumpVelocity * Time.deltaTime;

//            // 应用垂直移动
//            Vector3 motion = new Vector3(0, verticalMove, 0);
//            _player._CharacterController.Move(motion);

//            // 检查是否开始下落
//            if (currentJumpVelocity <= 0)
//            {
//                isJumping = false;
//            }
//        }

//        if (CheckAnimatorStateName("JumpStart", out float time) && time >= 0.9f)
//        {
//            _player.ChangeState(PlayerStateType.AirDown);
//        }
//    }

//    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
//    {
//        // 只应用水平位移，y轴由代码控制
//        Vector3 horizontalMotion = new Vector3(deltaPosition.x, 0, deltaPosition.z);
//        _player._CharacterController.Move(horizontalMotion);
//    }

//    public override void Exit()
//    {
//        _player._PlayerModle.ClearRootMotionAction();
//        _player._CharacterController.detectCollisions = true;
//    }
//}
public class PlayerJumpState : PlayerStateBase
{
    private float velocity;
    Vector3 verticalDisplancement;
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");

    public override void Enter()
    {
        #region 单跳跃状态
        //_player.PlayAnimation("Jump");
        #endregion

        #region 多跳跃状态

        velocity = _player.jumpStartSpeed;
        _player.PlayAnimation("JumpStart");
        _player._PlayerModle.SetRootMotionAction(OnRootMotion);

        #endregion
    }

    public override void Update()
    {
        #region 重力模拟模块
        bool isGrounded = Physics.SphereCast(_player.transform.position, 0.01f, Vector3.down, out RaycastHit hit, 0.02f, groundLayerMask);
        if(!isGrounded)
        {
            velocity += _player._gravity * Time.deltaTime;
            
        }
        else
        {
            velocity = 0;
        }

        float displancement = velocity * Time.deltaTime;
        verticalDisplancement = new Vector3(0, displancement, 0);
        _player._CharacterController.Move(verticalDisplancement);
        #endregion

        #region 多跳跃状态
        //跳跃分阶段时：空中持续、跳跃结束时的逻辑
        //if (CheckAnimatorStateName("JumpStart", out float time) && time >= 0.9f)
        //{
        //     _player.ChangeState(PlayerStateType.AirDown);
        //}
        #endregion


        #region Jump动画只有一个,暂时不用
        AnimatorStateInfo stateInfo = _player._PlayerModle._Animator.GetCurrentAnimatorStateInfo(0);
        
        //jump动画为一体的逻辑
        if (stateInfo.IsName("JumpStart"))
        {
            float animationTime = stateInfo.normalizedTime;
            //if(animationTime > 0.3f)
            //{
            //    _player.ChangeState(PlayerStateType.Idle);
            //    return;
            //}
            if (animationTime > 0f && animationTime < 0.3f)
            {

                float vertical = Input.GetAxis("Vertical");
                float horizontal = Input.GetAxis("Horizontal");
                if (vertical != 0 || horizontal != 0)
                {
                    Vector3 input = new Vector3(horizontal, 0, vertical);
                    float y = Camera.main.transform.rotation.eulerAngles.y;
                    Vector3 targetDir = Quaternion.Euler(0, y, 0) * input;
                    _player._PlayerModle.transform.rotation = Quaternion.Slerp(_player._PlayerModle.transform.rotation,
                        Quaternion.LookRotation(targetDir), Time.deltaTime * _player._rotationSpeed);
                }

                Vector3 direction = Camera.main.transform.TransformDirection(new Vector3(horizontal, 0, vertical));
                _player._CharacterController.Move(direction * Time.deltaTime * _player.moveSpeedForJump);
            }
            else if (animationTime >= 0.3f || isGrounded || _player._CharacterController.isGrounded)
            {
                _player.ChangeState(PlayerStateType.Idle);
                return;
            }
        }
        #endregion
    }
    public override void Exit()
    {
        //isJumping = false;
        velocity = 0f;
        verticalDisplancement = Vector3.zero;
        //horizontalMotion = Vector3.zero;
        _player._PlayerModle.ClearRootMotionAction();
    }

    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        #region 多跳跃状态运动控制相关

        ///*跳跃分阶段时：空中持续、跳跃结束时的角色运动相关逻辑*/
        //Vector3 horizontalMotion = new Vector3(deltaPosition.x, verticalDisplancement.y, deltaPosition.z);
        //Vector3 offset = moveStatePower * Time.deltaTime * _player.moveSpeedForJump * _player._PlayerModle.transform.forward;
        //Debug.Log(offset);
        //_player._CharacterController.Move(horizontalMotion + offset);
        /*
         */

        #endregion


        #region 单跳跃状态运动控制相关
        //不分阶段
        //deltaPosition.y *= _player.jumpStartSpeed;
        //deltaPosition.y += verticalDisplancement.y;
        _player._CharacterController.Move(deltaPosition);
        #endregion

    }
}

