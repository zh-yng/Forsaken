using UnityEngine;
public class BossLaserAttackState : State
{
    private BossStateMachine bossContext;

    public BossLaserAttackState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }

    public override void EnterState()
    {
        Debug.Log("laser attack");
        bossContext.Anim.SetTrigger("laser");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("laser");
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.LasersFinished == 1)
        {
            Debug.Log("finished");
            SwitchState(new BossIdleState(bossContext));
        }
    }

}