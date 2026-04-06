using UnityEngine;
public class BossLaserWindupState : State
{
    private BossStateMachine bossContext;
    private Transform firePoint;
    private LineRenderer laser;
    public BossLaserWindupState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }

    public override void EnterState()
    {
        //bossContext.flashCharacter();
        bossContext.LasersFinished = 0;
        bossContext.Anim.SetTrigger("laser_windup");
        firePoint = bossContext.Sprite.transform.Find("Broadsword").Find("ShootPoint");
        if (firePoint != null)
        {
            laser = firePoint.GetComponent<LineRenderer>();
            laser.material.SetColor("_BaseColor", new Color(255, 255, 255, 0.25f));
            laser.material.DisableKeyword("_EMISSION");
        }
    }

    public override void UpdateState()
    {
        Vector3 direction = (bossContext.Flipped ? -1 : 1) * Vector2.right;
        // Update laser (line renderer) to display properly.
        if (laser != null)
        {
            laser.SetPosition(0, firePoint.position);
            
            RaycastHit2D ray = Physics2D.Raycast(firePoint.position, direction, 25);

            if (ray)
            {
                laser.SetPosition(1, ray.point);
            } else
            {
                laser.SetPosition(1, firePoint.position + direction * 25);
            }

            laser.enabled = true;
            laser.SetPosition(1, firePoint.position + direction * 25);
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("laser_windup");
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.WindUpFinished)
        {
            SwitchState(new BossLaserState(bossContext));
        }
    }

}