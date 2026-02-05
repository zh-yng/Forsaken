using UnityEngine;
public class PlayerHurtState : State
{
    private PlayerStateMachine playerContext;
    public PlayerHurtState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.CanMove = false;
        playerContext.Anim.Play("Hurt");
        playerContext.AppliedMovementX = 0f;
        playerContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        playerContext.IsHurt = false;
        playerContext.CanMove = true;
    }

    public override void CheckSwitchStates()
    {
        if (!playerContext.HurtFinished)
        {
            return;
        }
        playerContext.HurtFinished = false;
        if (playerContext.IsHitPressed)
        {
            SwitchState(new PlayerAttackState(playerContext));
        }
        else if (playerContext.IsRunPressed)
        {
            SwitchState(new PlayerDashState(playerContext));
        } else if (playerContext.IsMovementPressed)
        {   
            SwitchState(new PlayerWalkState(playerContext));
        } else
        {
            SwitchState(new PlayerIdleState(playerContext));
        }
    }
}
