using UnityEngine;

public class PlayerJumpState : State
{
    private PlayerStateMachine playerContext;
    public PlayerJumpState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.CanMove = false;
        playerContext.Anim.Play("Jump");
        playerContext.Grounded = false;
        playerContext.RB.AddForce(Vector2.up * playerContext.JumpForce, ForceMode2D.Impulse);
        playerContext.AppliedMovementX = 0f;
        playerContext.AppliedMovementY = 0f;
        playerContext.IsJumpPressed = false; 
    }
    public override void UpdateState()
    {
        playerContext.AppliedMovementY = 0f ;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        playerContext.CanMove = true;
        playerContext.AppliedMovementY = 0f;
    }

    public override void CheckSwitchStates()
    {
        if (playerContext.IsHurt)
        {
            SwitchState(new PlayerHurtState(playerContext));
        } else if (playerContext.IsDashPressed && playerContext.CanDash)
        {
            SwitchState(new PlayerDashState(playerContext));
        } else if (playerContext.Grounded && !playerContext.IsMovementPressed )
        {
            SwitchState(new PlayerIdleState(playerContext));
        } else if (playerContext.Grounded && !playerContext.IsRunPressed)
        {
            SwitchState(new PlayerWalkState(playerContext));
        } else if (playerContext.Grounded && playerContext.IsRunPressed)
        {
            SwitchState(new PlayerDashState(playerContext));
        }
    }
}
