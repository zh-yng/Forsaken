using UnityEngine;
public class TestEvaMoveToHideState : State
{
    private TestEvaStateMachine evaContext;
    public TestEvaMoveToHideState(TestEvaStateMachine currentContext) : base(currentContext)
    {
        evaContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        evaContext.Anim.Play("Walk");
    }
    
    public override void UpdateState()
    {
        if (evaContext.TargetHideSpot == null) return;

        Vector3 target = evaContext.TargetHideSpot.position;
        Vector3 currentPos = evaContext.transform.position;

        Vector3 direction = (target - currentPos).normalized;
        evaContext.AppliedMovementX = direction.x * evaContext.MoveSpeed;

        CheckSwitchStates();
    }

    public override void CheckSwitchStates()
    {
        float distance = Mathf.Abs(evaContext.transform.position.x - evaContext.TargetHideSpot.position.x);
        if (distance < 0.1f)
        {
            SwitchState(new TestEvaHiddenState(evaContext));
        }
    }
    public override void ExitState()
    {
    }
}
