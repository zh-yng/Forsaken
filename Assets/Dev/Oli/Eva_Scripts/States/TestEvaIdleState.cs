using UnityEngine;
public class TestEvaIdleState : State
{
    private TestEvaStateMachine evaContext;
    public TestEvaIdleState(TestEvaStateMachine currentContext) : base(currentContext)
    {
        evaContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        evaContext.Anim.Play("Idle");
        evaContext.AppliedMovementX = 0f;
        evaContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (evaContext.CurrentState is TestEvaMoveToHideState || evaContext.CurrentState is TestEvaHiddenState)
        {
            return; 
        }
        if (evaContext.FollowRange())
        {
            SwitchState(new TestEvaFollowState(evaContext));
        }
    }
}
