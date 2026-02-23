using UnityEngine;
public class BossSummonState : State
{
    private BossStateMachine bossContext;

    public BossSummonState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }

    public override void EnterState(){
        Debug.Log("summons attack");
        bossContext.Anim.SetTrigger("summon");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        bossContext.LastDroneSummon = Time.time;
        bossContext.Anim.ResetTrigger("summon");
    }

    //fill in transition logic
    public override void CheckSwitchStates()
    {
        SwitchState(new BossIdleState(bossContext));
    }

}