using UnityEditor.Rendering;
using UnityEngine;

public class BossMeleeAttackState : State
{
    private BossStateMachine bossContext;
    public BossMeleeAttackState(BossStateMachine currentContext) : base(currentContext)
    {
        
        bossContext = currentContext;
    }
    public override void EnterState()
    {
        bossContext.AttackFinished = 0;
        bossContext.Anim.SetTrigger("melee");
        bossContext.AppliedMovementX = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        bossContext.AttackFinished = 1;
        bossContext.Anim.ResetTrigger("melee");
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.AttackFinished == 1)
        {
            SwitchState(new BossIdleState(bossContext));
        }
    }
}
