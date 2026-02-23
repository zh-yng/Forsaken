using UnityEditor.Rendering;
using UnityEngine;

public class DogPounceState : State
{
    private DogStateMachine dogContext;
    public DogPounceState(DogStateMachine currentContext) : base(currentContext)
    {
        
        dogContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        Vector3 target = new Vector3(dogContext.Player.gameObject.transform.position.x, dogContext.RB.gameObject.transform.position.y, 0f);
        Vector3 currentPos = new Vector3(dogContext.RB.gameObject.transform.position.x, dogContext.RB.gameObject.transform.position.y, 0f);
        Vector3 direction = (target - currentPos).normalized;

        dogContext.InAttack = true;
        dogContext.OnGround = false;
        dogContext.RB.AddForce(new Vector2(direction.x * dogContext.JumpForce.x, dogContext.JumpForce.y), ForceMode2D.Impulse);
        dogContext.AppliedMovementX = 0;
        dogContext.AppliedMovementY = 0;
    }
    public override void UpdateState()
    {
        dogContext.AppliedMovementY = 0f;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        dogContext.InAttack = false;
    }

    public override void CheckSwitchStates()
    {
        if (dogContext.OnGround)
        {
            SwitchState(new DogStunState(dogContext));
        }
    }
}
