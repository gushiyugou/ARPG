using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;


public class PlayerIdleState : PlayerStateBase
{
    private LayerMask groundLayerMask = LayerMask.GetMask("Env");
    //private float gravity = 0f;
    public override void Enter()
    {
        _player._PlayerModle._Animator.SetBool("IsIdle",true);
        //_player.PlayAnimationImmediately("Idle");
        //_player._CharacterController.detectCollisions = false;
    }

    public override void Update()
    {
        //UpdataGravity();


        //TODO:检测攻击
        if (Input.GetMouseButtonDown(0))
        {
            _player.ChangeState(PlayerStateType.AtkNormal1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            moveStatePower = 0;
            
            _player.ChangeState(PlayerStateType.Jump);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //moveStatePower = 0;
            _player.ChangeState(PlayerStateType.SidestepReverse);
            return;
        }

        //检测玩家移动
        //_player._CharacterController.Move(new Vector3(0, velocity * Time.deltaTime, 0));
        bool isCast = Physics.CheckSphere(_player.transform.position, 0.2f, groundLayerMask);
        //if (!_player._CharacterController.isGrounded && !isCast)
        //{
        //    Debug.Log("idle检测的isGround：" + _player._CharacterController.isGrounded);
        //    _player.ChangeState(PlayerStateType.AirDown);
        //    return;
        //}
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if(horizontal != 0f || vertical != 0f)
        {
            _player.ChangeState(PlayerStateType.Moevment);
            return;
        }
        //if (isCast)
        //{
        //    //UpdataGravity();
        //}
        //else
        //{
        //    BeseGravityVelocity = 0;
        //}
    }

    public override void Exit()
    {
        
        _player._PlayerModle._Animator.SetBool("IsIdle", false);
        _player._PlayerModle._Animator.applyRootMotion = true;
        //_player._CharacterController.detectCollisions = true;
        //SyncModelToController();
    }
}
