using UnityEngine;
public class BossBeginSummonsState : State
{
    private BossStateMachine bossContext;
    public BossBeginSummonsState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        
        bossContext.Anim.SetTrigger("walk");
        
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("walk");
    }

    public override void CheckSwitchStates()
    {
        SwitchState(new BossSummonState(bossContext));
    }
}
