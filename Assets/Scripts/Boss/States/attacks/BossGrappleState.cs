using UnityEngine;
using System.Collections;

public class BossGrappleState : State
{
    private BossStateMachine bossContext;
    private LineRenderer lineRenderer;
    private Transform chainStart;

    public BossGrappleState(BossStateMachine currentContext) : base(currentContext)
    {
        bossContext = currentContext;
    }

    public override void EnterState()
    {
        bossContext.GrapplingFinished = 0;
        bossContext.Anim.SetTrigger("grapple");

        lineRenderer = bossContext.GetComponentInChildren<LineRenderer>(true);
        if (lineRenderer == null)
        {
            Debug.Log("LineRenderer component not found on boss GameObject");
            SwitchState(new BossTransitionState(bossContext));
            return;
        }
        lineRenderer.gameObject.SetActive(true);
        chainStart = lineRenderer.transform;

        bossContext.StartCoroutine(AnimateGrapple());
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        bossContext.Anim.ResetTrigger("grapple");
        lineRenderer.gameObject.SetActive(false);
    }

    public override void CheckSwitchStates()
    {
        if (bossContext.GrapplingFinished == 1)
        {
            SwitchState(new BossIdleState(bossContext));
        }
    }

    private IEnumerator AnimateGrapple()
    {
        float elapsed = 0f;
        float duration = bossContext.GrappleDuration;
        float stopDistance = 2f;

        // Jump up before throwing the chain
        float jumpHeight = 5f;
        Vector3 jumpTarget = bossContext.transform.position + Vector3.up * jumpHeight;
        while (bossContext.transform.position.y < jumpTarget.y)
        {
            bossContext.transform.position = Vector3.MoveTowards(bossContext.transform.position, jumpTarget, bossContext.GrappleSpeed * Time.deltaTime);
            yield return null;
        }

        // The throwing of the chain
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            Vector3 chainTip = Vector3.Lerp(chainStart.position, bossContext.Player.transform.position, percent);

            lineRenderer.SetPosition(0, chainStart.position);
            lineRenderer.SetPosition(1, chainTip);
            yield return null;
        }

        // The pulling of the boss towards the grapple target
        while (Vector3.Distance(bossContext.transform.position, bossContext.Player.transform.position) > stopDistance)
        {
            lineRenderer.SetPosition(0, bossContext.GetComponent<Collider2D>().bounds.center);
            lineRenderer.SetPosition(1, bossContext.Player.transform.position);
            bossContext.transform.position = Vector3.MoveTowards(bossContext.transform.position, bossContext.Player.transform.position, bossContext.GrappleSpeed * Time.deltaTime);
            yield return null;
        }
        bossContext.GrapplingFinished = 1;
    }

}