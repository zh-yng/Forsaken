using UnityEngine;

public class PlayerIdleState : State
{
    private PlayerStateMachine playerContext;
    public PlayerIdleState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.CanMove = true;
        playerContext.Anim.SetTrigger("idle");
        playerContext.AppliedMovementX = 0f;
        playerContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        playerContext.Anim.ResetTrigger("idle");
    }

    public override void CheckSwitchStates()
    {
        if (playerContext.IsHurt)
        {
            SwitchState(new PlayerHurtState(playerContext));
        }
        else if (playerContext.IsHitPressed)
        {
            SwitchState(new PlayerAttackState(playerContext));
        }
        else if (playerContext.IsShootPressed && playerContext.IsAimingForward)
        {
            SwitchState(new PlayerShootState(playerContext));
        } else if (playerContext.Grounded && playerContext.IsJumpPressed)
        {
            SwitchState(new PlayerJumpState(playerContext));
        } else if (playerContext.IsRunPressed)
        {
            SwitchState(new PlayerDashState(playerContext));
        } else if (playerContext.IsMovementPressed)
        {   
            SwitchState(new PlayerWalkState(playerContext));
        } else if (playerContext.IsBlocking) {
            SwitchState(new PlayerBlockState(playerContext));
        }
    }
}
