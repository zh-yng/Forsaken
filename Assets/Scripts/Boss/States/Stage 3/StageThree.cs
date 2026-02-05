using UnityEngine;

public class StageThree : State
{
    private BossStateMachine bossContext;
    public StageThree(BossStateMachine currentContext) : base(currentContext)
    {
        
        bossContext = currentContext;
        isBaseState = true;
        InitializeSubStates();
    }
    public override void InitializeSubStates()
    {
        if (bossContext.GrappleInRange())
        {
            SetSubState(new BossGrappleState(bossContext));
        }
        else if (bossContext.canDash())
        {
            SetSubState(new BossDashWindupState(bossContext));
        }
        else if (bossContext.InRange())
        {
            SetSubState(new BossAttackState(bossContext));
        }
        else if (bossContext.IsStunned)
        {
            SetSubState(new BossStunState(bossContext));
        }
        else
            SetSubState(new BossWalkState(bossContext));
    }
    public override void EnterState()
    {
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.IsTransitioning)
        {
            SwitchState(new BossTransitionState(bossContext));
        }
        
    }
}
