using UnityEngine;

public class PlayerDashAttackState : State
{
    private PlayerStateMachine playerContext;
    private Vector3 endGoal;
    private Vector3 direction;
    public PlayerDashAttackState(PlayerStateMachine currentContext) : base(currentContext)
    {
        playerContext = currentContext;
    }
    public override void EnterState()
    {
        direction = new Vector3(playerContext.IsMovementPressed ? playerContext.CurrentMovementInput.x : playerContext.Sprite.localScale.x, 0f, 0f);
        playerContext.Sprite.localScale = new Vector3(direction.x, 1f, 1f);
        endGoal = playerContext.Player.transform.position + direction * playerContext.DashDistance;
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(6, 8, true);
        playerContext.Anim.SetTrigger("dash");
        playerContext.DashTrail.GetComponent<DashTrail>().enabled = true;
        playerContext.DashTrail.GetComponent<DashTrail>().IsDrawingTrail = true;
        playerContext.DashTrail.GetComponent<DashTrail>().Direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
    }
    public override void UpdateState()
    {
        Vector2 newPos = Vector2.MoveTowards(playerContext.Player.transform.position, endGoal, playerContext.DashSpeed * Time.fixedDeltaTime);
        if (Vector2.Distance(newPos, endGoal) <= 0.001)
        {
            playerContext.DashFinished = true;
            CheckSwitchStates();
        } else {
            playerContext.RB.MovePosition(newPos);
        }
    }
    public override void ExitState()
    {
        playerContext.DashFinished = false;
        playerContext.IsDashing = false;
        playerContext.Anim.ResetTrigger("dash");
        playerContext.DashTrail.GetComponent<DashTrail>().IsDrawingTrail = false;
        playerContext.DashTrail.GetComponent<DashTrail>().Clear();
        playerContext.DashTrail.GetComponent<DashTrail>().enabled = false;
        Physics2D.IgnoreLayerCollision(6, 7, false);
        Physics2D.IgnoreLayerCollision(6, 8, false);
        
    }

    public override void CheckSwitchStates()
    {
        if (playerContext.IsRunPressed)
        {
            SwitchState(new PlayerRunState(playerContext));
        } else if (playerContext.IsMovementPressed)
        {
            SwitchState(new PlayerWalkState(playerContext));
        } else 
        {
            SwitchState(new PlayerIdleState(playerContext));
        }
    }
}
