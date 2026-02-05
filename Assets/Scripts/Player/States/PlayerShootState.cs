using UnityEngine;

public class PlayerShootState : State
{
    private PlayerStateMachine playerContext;
    public PlayerShootState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.Anim.Play("Shoot");
        playerContext.AppliedMovementX = 0f;
        playerContext.AppliedMovementY = 0f;
        playerContext.IsShootPressed = false;  // Reset to prevent continuous shooting

    }
    public override void UpdateState()
    {
        // Call Shoot on the ranged weapon
        Player_Ranged rangedWeapon = playerContext.GetComponentInChildren<Player_Ranged>();
        if (rangedWeapon != null && playerContext.ShootStarted)
        {
            rangedWeapon.Shoot();
            playerContext.ShootStarted = false;
        }
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
        else if (!playerContext.ShootFinished)
        {
            return;
        }
        playerContext.ShootFinished = false; 
        if (playerContext.IsRunPressed)
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
