using UnityEngine;
public class BossStartState : State
{
    private BossStateMachine bossContext;
    private float curTime;
    public BossStartState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        bossContext.Anim.Play("Idle");
        bossContext.AppliedMovementX = 0f;
        bossContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.AppliedMovementX = 0f;
        bossContext.AppliedMovementY = 0f;
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.FightStarted)
        {
            SwitchState(new BossTransitionState(bossContext));
        
        } 
    }
}
