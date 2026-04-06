using UnityEngine;
public class BossTeleportState : State
{
    private BossStateMachine bossContext;
    private GameObject hue;
    private float timer = 0;
    private float timeThreshold = 1;
    public BossTeleportState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
        hue = GameObject.Find("HUE");
    }
    public override void EnterState()
    {
        Physics2D.IgnoreLayerCollision(6, 7, true);
        Physics2D.IgnoreLayerCollision(7, 8, true);
        //bossContext.Anim.SetTrigger("teleport");
        if (hue.transform.position.x > -4)
        {
            hue.transform.position = new Vector3(-19, 0, 0);
        }
        else
        {
            hue.transform.position = new Vector3(13, 0, 0);
        }
    }
    public override void UpdateState()
    {
        timer += Time.deltaTime;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        //bossContext.Anim.ResetTrigger("teleport");
        Physics2D.IgnoreLayerCollision(6, 7, false);
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    //fill in transition logic
    public override void CheckSwitchStates()
    {
        if (timer >= timeThreshold) {
            SwitchState(new BossIdleState(bossContext));
        }
    }
}
