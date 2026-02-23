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
        Debug.Log("entering hurt");
        playerContext.CanMove = false;
        playerContext.Anim.SetTrigger("hurt");
        playerContext.AppliedMovementX = 0f;
        playerContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        if (playerContext.HurtFinished)
        {
            CheckSwitchStates();
        }
        
    }
    public override void ExitState()
    {
        playerContext.Anim.ResetTrigger("hurt");
        playerContext.HurtFinished = false;
        playerContext.IsHurt = false;
        playerContext.CanMove = true;
    }

    public override void CheckSwitchStates()
    {
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
