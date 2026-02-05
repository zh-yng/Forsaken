using UnityEditor.Rendering;
using UnityEngine;

public class BossAttackState : State
{
    private BossStateMachine bossContext;
    public BossAttackState(BossStateMachine currentContext) : base(currentContext)
    {
        
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        bossContext.AttackFinished = 0;
        bossContext.Anim.Play("Attack");
        bossContext.AppliedMovementX = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.AttackFinished = 1;
    }

    public override void CheckSwitchStates()
    {
        Debug.Log(bossContext.canDash());
        if (bossContext.IsStunned)
        {   Debug.Log("switching states");
            SwitchState(new BossStunState(bossContext));
        }
        else if (bossContext.CurrentStage == 3 && bossContext.canDash())
        {
            SwitchState(new BossDashWindupState(bossContext));
        }
        else if (bossContext.AttackFinished == 1)
        {
            SwitchState(new BossWalkState(bossContext));
        }
    }
}
