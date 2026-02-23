using UnityEngine;
public class BossChargedDashState : State
{
    private BossStateMachine bossContext;
    public BossChargedDashState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        bossContext.AttackFinished = 0;
        bossContext.Anim.SetTrigger("dashAttack");
        bossContext.AppliedMovementX = ((bossContext.Flipped ? -1 : 1)) * bossContext.MoveSpeed * 3;
        bossContext.LastDashTime = Time.time;
        Debug.Log("This is existing");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("dashAttack");
        bossContext.AttackFinished = 1;
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.AttackFinished == 1)
        {
            SwitchState(new BossIdleState(bossContext));
        }
    }
}
