using UnityEngine;

public class CrowPounceState : State
{
    private CrowStateMachine crowContext;
    public CrowPounceState(CrowStateMachine currentContext) : base(currentContext)
    {
        
        crowContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        crowContext.InAttack = true;
        Debug.Log("cro hunt");
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        crowContext.InAttack = false;
    }

    public override void CheckSwitchStates()
    {
        if (!crowContext.InAttack) {
            SwitchState(new CrowRecoverState(crowContext));
        }   
    }
}
