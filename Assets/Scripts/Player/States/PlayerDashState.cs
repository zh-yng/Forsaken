using UnityEngine;

public class PlayerDashState : State
{
    private PlayerStateMachine playerContext;
    public PlayerDashState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
        InitializeSubStates();
    }
    public override void InitializeSubStates()
    {
        if (playerContext.CanDash)
        {
            SetSubState(new PlayerDashAttackState(playerContext));
        } else
        {
            SetSubState(new PlayerRunState(playerContext));
        }
    }
    public override void EnterState()
    {
        playerContext.DashFinished = false;
        playerContext.IsDashing = true;
        playerContext.CanMove = false;
        // playerContext.AppliedMovementX = 0f;
        // playerContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        playerContext.CanMove = true;
        playerContext.IsDashing = false;
    }

    public override void CheckSwitchStates()
    {
        if (!playerContext.IsMovementPressed && playerContext.DashFinished)
        {
            SwitchState(new PlayerIdleState(playerContext));
        } else if (!playerContext.IsRunPressed && playerContext.IsMovementPressed && playerContext.DashFinished)
        {
            SwitchState(new PlayerWalkState(playerContext));
        }
    }
}
