
using UnityEngine;
public class BossSummonState : State
{
    private BossStateMachine bossContext;
    private GameObject attackDog;
    private Transform t;

    public BossSummonState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
        attackDog = bossContext.AttackDog;
        t = bossContext.RB.gameObject.transform;
        Debug.Log("Attemping Summon State");
    }

    public override void EnterState(){
        GameObject dog;
        bossContext.CurEnemies += 1;
        Debug.Log("summons attack");
        bossContext.Anim.SetTrigger("summon");
        if (attackDog != null) {
            dog = Object.Instantiate(attackDog, t.position, t.rotation);
            dog.GetComponent<DogStateMachine>().AggroDistance = Mathf.Infinity;
        } else {
            Debug.Log("Attack Dog not assigned!");
        }
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