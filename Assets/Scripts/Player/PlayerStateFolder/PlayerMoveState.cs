using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private enum MoveChildState
    {
        Move,
        RunStop,
        WalkStop
    }
    private float walkToRunTransition;
    private MoveChildState moveState;
    private MoveChildState MoveState
    {
        get => moveState;
        set
        {
            moveState = value;
            //子状态进入逻辑
            switch (moveState)
            {
                case MoveChildState.Move:
                    _player.PlayAnimation("Move", 0.15f);
                    
                    break;
                case MoveChildState.RunStop:
                    _player.PlayAnimation("RunStop",0.25f);
                    break;
                case MoveChildState.WalkStop:
                    _player.PlayAnimation("WalkStop",0.25f);
                    break;
            }
        }
    }
    public override void Enter()
    {
        //TODO Move状态下的攻击
        if (Input.GetMouseButtonDown(0))
        {
            _player.ChangeState(PlayerStateType.AtkNormal1);
            return;
        }

        //_player._PlayerModle._Animator.applyRootMotion = true;
        //AudioManager.Instance.AddAudioClip("RunStop",)
        _player._PlayerModle.SetRootMotionAction(OnRootMotion);
        MoveState = MoveChildState.Move;
        //注册根运动s
        //_player._PlayerModle.SetRootMotionAction(OnRootMotion);
    }

    public override void Update()
    {
        _player._CharacterController.Move(new Vector3(0, _player._gravity * Time.deltaTime, 0));
        if (Input.GetMouseButtonDown(0))
        {
            _player.ChangeState(PlayerStateType.AtkNormal1);
            return;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            switch (moveState)
            {
                case MoveChildState.Move:
                    _player.ChangeState(PlayerStateType.Sidestep);
                    break;
                case MoveChildState.RunStop:
                    _player.ChangeState(PlayerStateType.SidestepReverse);
                    break;
                case MoveChildState.WalkStop:
                    _player.ChangeState(PlayerStateType.SidestepReverse);
                    break;
            }
            
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //通过控制速度状态的值来动态赋值jumpPower的值0
            moveStatePower = walkToRunTransition + 1;
            _player.ChangeState(PlayerStateType.Jump);
            return;
        }
        switch (moveState)
        {
            case MoveChildState.Move:
                MoveOnUpdata();
                break;
            case MoveChildState.RunStop:
                RunStopOnUpdata();
                break;
            case MoveChildState.WalkStop:
                WalkStopOnUpdata();
                break;
        }
        //if (!_player._CharacterController.isGrounded)
        //{
        //    Debug.Log(_player._CharacterController.isGrounded);
        //    moveStatePower = walkToRunTransition + 1;
        //    _player.ChangeState(PlayerStateType.AirDown);
        //    return;
        //}



    }

    private void MoveOnUpdata()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float rawH = Input.GetAxisRaw("Horizontal");
        float rawV = Input.GetAxisRaw("Vertical");


        

        if (rawH == 0 && rawV == 0)
        {
            if(walkToRunTransition < 0.4f)
            {
                MoveState = MoveChildState.WalkStop;
                return;
            }
            else if (walkToRunTransition > 0.6f)
            {
                MoveState = MoveChildState.RunStop;
                return;
            }
            else if (horizontal == 0 && vertical == 0)
            {
                _player.ChangeState(PlayerStateType.Idle);
                return;
            }
            
            //_player.ChangeState(PlayerStateType.Idle);
        }
        else
        {
            //处理走到跑的过渡
            if (Input.GetKey(KeyCode.LeftShift))//走到跑
            {
                walkToRunTransition = Mathf.Clamp(
                    walkToRunTransition + Time.deltaTime * _player.walkToRunTransition, 0, 1);
            }
            else //跑到走
            {
                walkToRunTransition = Mathf.Clamp(
                   walkToRunTransition - Time.deltaTime * _player.walkToRunTransition, 0, 1);
            }

            _player._PlayerModle._Animator.SetFloat("Move", walkToRunTransition);

            //通过修改动画播放速度，来达到实际的位移距离变化
            _player._PlayerModle._Animator.speed = Mathf.Lerp(_player.walkSpeed, _player.RunSpeed, walkToRunTransition);

            if(horizontal != 0 || vertical != 0)
            {
                //处理旋转的问题
                Vector3 input = new Vector3(horizontal, 0, vertical);
                float y = Camera.main.transform.eulerAngles.y;

                //让四元数与向量相乘，表示让这个向量按照这个四元数所表达的角度进行旋转后得到新的向量
                Vector3 targetDiretion = Quaternion.Euler(0, y, 0) * input;
                _player._PlayerModle.transform.rotation = Quaternion.Slerp(
                    _player._PlayerModle.transform.rotation, Quaternion.LookRotation(targetDiretion),
                    Time.deltaTime * _player._rotationSpeed);
            }
        }
    }


    private void RunStopOnUpdata()
    {
        
        if (CheckAnimatorStateName("RunStop",out float animationTime))
        {
            
            if (animationTime >= 0.75f)
                _player.ChangeState(PlayerStateType.Idle);
        }
    }
    private void WalkStopOnUpdata()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            //moveStatePower = 0;
            _player.ChangeState(PlayerStateType.SidestepReverse);
            return;
        }
        if (CheckAnimatorStateName("WalkStop", out float animationTime))
        {
            if (animationTime >= 0.5f)
                _player.ChangeState(PlayerStateType.Idle);
        }
    }
    public override void Exit()
    {
        walkToRunTransition = 0f;
        _player._PlayerModle._Animator.speed = 1;
        _player._PlayerModle.ClearRootMotionAction();
    }

    //private IEnumerator DelayedDisableRootMotion()
    //{
    //    // 等待动画过渡完成
    //    yield return new WaitForSeconds(0.1f);
    //    _player._PlayerModle._Animator.applyRootMotion = false;
    //}
    
    private void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y = _player._gravity * Time.deltaTime;
        _player._CharacterController.Move(deltaPosition);
    }
}