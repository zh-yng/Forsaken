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
        bossContext.WindUpFinished = false;
        bossContext.flashCharacter();
        bossContext.Anim.Play("Charge Wind Up");
        bossContext.AppliedMovementX = 0;
        bossContext.LastDashTime = Time.time;
        Debug.Log("Attempting a dash here!");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.WindUpFinished = true;
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.IsStunned)
        {
            Debug.Log("switching states");
            SwitchState(new BossStunState(bossContext));
        }
        else if (bossContext.WindUpFinished == true)
        {
            SwitchState(new BossChargedDashState(bossContext));
        }
    }
}
