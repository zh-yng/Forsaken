using UnityEngine;
public class BossDashWindupState : State
{
    private BossStateMachine bossContext;
    public BossDashWindupState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        bossContext.AttackFinished = 0;
        bossContext.IsDashing = true;
        bossContext.WindUpFinished = false;
        bossContext.Anim.SetTrigger("charge");
        bossContext.AppliedMovementX = 0;
        bossContext.LastDashTime = Time.time;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.IsDashing = false;
        bossContext.WindUpFinished = true;
        bossContext.Anim.ResetTrigger("charge");
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.IsStunned)
        {
            SwitchState(new BossStunState(bossContext));
        }
        else if (bossContext.WindUpFinished == true)
        {
            SwitchState(new BossChargedDashState(bossContext));
        }
    }
}
