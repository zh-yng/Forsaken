using UnityEngine;

public class Boss_Ranged : Weapon
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 15f;

    protected override void Init()
    {
        weilder = GameObject.FindGameObjectWithTag("Boss").transform;
        victim = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;
        Vector3 target = new Vector3(victim.position.x, firePoint.position.y, 0f);

        Vector3 shootDir = (target - firePoint.position).normalized;

        GameObject bulletObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        if (bulletObj.TryGetComponent(out BossBullet bullet))
        {
            if (weilder.GetComponent<StateMachine>().IsParryStunned)
            {
                bullet.Initialize(shootDir, bulletSpeed / victim.GetComponent<PlayerStateMachine>().ParrySlowDownAmount);
            } else
            {
                bullet.Initialize(shootDir, bulletSpeed);
            }
            
        }
    }
}