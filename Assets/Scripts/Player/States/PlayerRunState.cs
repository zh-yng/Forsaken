using UnityEngine;

public class PlayerRunState : State
{
    private PlayerStateMachine playerContext;
    public PlayerRunState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
    }
    public override void EnterState()
    {
        playerContext.CanMove = true;
        playerContext.Anim.Play("Run");
        playerContext.AppliedMovementX = playerContext.CurrentMovementInput.x * playerContext.RunSpeed;
        
    }
    public override void UpdateState()
    {
        playerContext.AppliedMovementX = playerContext.CurrentMovementInput.x * playerContext.RunSpeed * 0.999f;
        
        CheckSwitchStates();
    }
    public override void ExitState()
    {
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
        } else if (!playerContext.IsMovementPressed)
        {
            playerContext.IsRunPressed = false;
            SwitchState(new PlayerIdleState(playerContext));
        } else if (playerContext.IsMovementPressed && !playerContext.IsRunPressed)
        {   
            SwitchState(new PlayerWalkState(playerContext));
        }
    }
}
