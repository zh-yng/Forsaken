using UnityEngine;

public class PlayerParryState : State
{
    private PlayerStateMachine playerContext;
    public PlayerParryState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        playerContext.CanParry = true;
        playerContext.Anim.SetTrigger("parry");
        //to-do: find a better way to trigger the stun state
        playerContext.Manager.PlayerParry();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {   
        playerContext.Anim.ResetTrigger("parry");
    }

    public override void CheckSwitchStates() {
        // if (playerContext.IsParrying) return;
        // if (playerContext.IsBlocking) {
        //     SwitchState(new PlayerBlockState(playerContext, false)); // don't allow repeated parries - reblock
        // }

        if (playerContext.IsMovementPressed) {
            if (playerContext.IsRunPressed) {
                SwitchState(new PlayerDashState(playerContext));
            }
            else {
                SwitchState(new PlayerWalkState(playerContext));
            }
        }
        
        else SwitchState(new PlayerIdleState(playerContext));
        
    }
}