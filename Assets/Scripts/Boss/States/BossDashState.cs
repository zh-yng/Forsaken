using UnityEngine;
public class BossDashState : State
{
    private BossStateMachine bossContext;
    public BossDashState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        bossContext.Anim.SetTrigger("dash");
        bossContext.LastDashMovementTime = Time.time;
        
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("dash");
    }

    //fill in transition logic
    public override void CheckSwitchStates()
    {
        SwitchState(new BossMeleeAttackState(bossContext));
    }
}
