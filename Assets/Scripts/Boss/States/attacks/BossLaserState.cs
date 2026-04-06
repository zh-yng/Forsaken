using UnityEngine;
public class BossLaserState : State
{
    private BossStateMachine bossContext;
    private Transform firePoint;
    private LineRenderer laser;
    private Transform hitBox;

    public BossLaserState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
        firePoint = bossContext.Sprite.transform.Find("Broadsword").Find("ShootPoint");
        if (firePoint != null)
        {
            laser = firePoint.GetComponent<LineRenderer>();
            laser.material.SetColor("_BaseColor", Color.white);
            laser.material.EnableKeyword("_EMISSION");
        }
        hitBox = bossContext.Sprite.Find("LaserHitbox");
    }

    public override void EnterState()
    {
        bossContext.AppliedMovementX = 0;
        bossContext.AppliedMovementY = 0;
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
        if (hitBox != null)
        {
            float length = Mathf.Abs(laser.GetPosition(1).x - laser.GetPosition(0).x);

            // Move hitbox to proper location of laser.
            hitBox.position = firePoint.position + direction * length / 2;
            hitBox.GetComponent<BoxCollider2D>().size = new Vector2(length / Mathf.Abs(hitBox.lossyScale.x), 0.1f / Mathf.Abs(hitBox.lossyScale.y));
            hitBox.GetComponent<BoxCollider2D>().enabled = true;
        }
        bossContext.Anim.Play("Laser");
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("exiting");
        if (laser != null)
        {
            laser.enabled = false;
            hitBox.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.LasersFinished == 1)
        {
            SwitchState(new BossIdleState(bossContext));
        }
    }

}