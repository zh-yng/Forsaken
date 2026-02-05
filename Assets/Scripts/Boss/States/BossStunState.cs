using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class BossStunState : State
{
    private BossStateMachine bossContext;
    private float curTime;
    public BossStunState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        Debug.Log("currently stunned");
        bossContext.Anim.Play("Idle");
        bossContext.AppliedMovementX = 0f;
        bossContext.AppliedMovementY = 0f;
        curTime = 0f;
    }
    public override void UpdateState()
    {
        curTime += Time.deltaTime;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.IsStunned = false;
    }

    public override void CheckSwitchStates()
    {
        if (curTime > bossContext.StunTime)
        {
            if (bossContext.CurrentStage >= 3 && bossContext.canDash())
            {
                SwitchState(new BossDashWindupState(bossContext));
            }
            if (bossContext.CurrentStage >= 2 && bossContext.GrappleInRange())
            {
                SwitchState(new BossGrappleState(bossContext));
            }
            else if (bossContext.InRange())
            {
                SwitchState(new BossAttackState(bossContext));
            } else
            {
                SwitchState(new BossWalkState(bossContext));
            }
        } 
    }
}
