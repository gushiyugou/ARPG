using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
public class BossStateBase : StateBase
{
    public BossController boss;
    public BossModle bossModle;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        boss = owner as BossController;
    }



    protected virtual bool CheckAnimatorStateName(string stateName, out float normalizedTime)
    {
        AnimatorStateInfo nextState = boss.Model._Animator.GetNextAnimatorStateInfo(0);
        if (nextState.IsName(stateName))
        {
            normalizedTime = nextState.normalizedTime;
            return true;
        }


        AnimatorStateInfo currentState = boss.Model._Animator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentState.normalizedTime;
        return currentState.IsName(stateName);
    }
}
