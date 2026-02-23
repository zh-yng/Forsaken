using UnityEngine;

public class PlayerWalkState : State
{
    private PlayerStateMachine playerContext;
    public PlayerWalkState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.CanMove = true;
        playerContext.Anim.SetTrigger("walk");
        playerContext.AppliedMovementX = playerContext.CurrentMovementInput.x * playerContext.MoveSpeed;
    }
    public override void UpdateState()
    {
        playerContext.AppliedMovementX = playerContext.CurrentMovementInput.x * playerContext.MoveSpeed * 0.999f;
        
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        playerContext.Anim.ResetTrigger("walk");
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
        }
        else if (playerContext.Grounded && playerContext.IsJumpPressed)
        {
            SwitchState(new PlayerJumpState(playerContext));
        }  else if (playerContext.IsMovementPressed && playerContext.IsRunPressed)
        {   
            SwitchState(new PlayerDashState(playerContext));
        } else if (!playerContext.IsMovementPressed )
        {
            SwitchState(new PlayerIdleState(playerContext));
        }
    }
}
