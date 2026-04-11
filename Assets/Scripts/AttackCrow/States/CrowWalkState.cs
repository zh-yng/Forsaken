using UnityEngine;
using UnityEngine.UIElements;
public class CrowWalkState : State
{
    private CrowStateMachine crowContext;
    float swoopTime = 1.5f;
    Vector3 startPos;
    Vector3 controlPoint;
    Vector3 endPos;
    float t;
    public CrowWalkState(CrowStateMachine currentContext) : base(currentContext)
    {
        crowContext = currentContext;
        isBaseState = true;

        
    }
    public override void EnterState()
    {
        crowContext.Anim.Play("Walk");
        startPos = new Vector3(crowContext.RB.gameObject.transform.position.x, crowContext.RB.gameObject.transform.position.y, 0f);
        endPos = new Vector3(crowContext.Player.gameObject.transform.position.x, 
            crowContext.Player.gameObject.transform.position.y, 0f);
        Debug.Log(endPos);
        controlPoint = Vector3.down * 2f + startPos;
        t = 0f;
    }
    public override void UpdateState()
    {
        if (t < 1) {
            t = Mathf.Min(t + Time.deltaTime / swoopTime, 1);
            crowContext.RB.gameObject.transform.position = Bezier(startPos, controlPoint, endPos, t);
        }
        Debug.Log(t);
        CheckSwitchStates();
    }
    Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return Mathf.Pow(1 - t, 2) * a +
               2 * (1 - t) * t * b +
               Mathf.Pow(t, 2) * c;
    }

    public override void ExitState()
    {
        crowContext.InAttack = false;
    }

    public override void CheckSwitchStates()
    {
        if (crowContext.IsStunned)
        {   
            SwitchState(new CrowStunState(crowContext));
        }
        if (t >= 1) {
            Debug.Log(crowContext.InRange() + ", Attack: " + !crowContext.InAttack);
            if (crowContext.InRange() && !crowContext.InAttack)
            {
                Debug.Log("We attack.");
                SwitchState(new CrowPounceState(crowContext));
            }
            else
            {
                Debug.Log("We recover.");
                SwitchState(new CrowRecoverState(crowContext));
            }
        }
        
    }
}
